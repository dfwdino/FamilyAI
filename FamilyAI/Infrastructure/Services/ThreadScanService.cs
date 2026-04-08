using FamilyAI.Domain.Data;
using FamilyAI.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FamilyAI.Infrastructure.Services
{
    public class ThreadScanService
    {
        private readonly MyDbContext _db;
        private readonly FlagRuleService _flagRuleService;
        private readonly OllamaSettingService _ollamaSettingService;

        private const string DefaultUrl = "http://localhost:11434/";
        private const string DefaultModel = "llama3.2-vision";

        public ThreadScanService(MyDbContext db, FlagRuleService flagRuleService, OllamaSettingService ollamaSettingService)
        {
            _db = db;
            _flagRuleService = flagRuleService;
            _ollamaSettingService = ollamaSettingService;
        }

        // ── Public API ────────────────────────────────────────────────────────────

        /// <summary>Runs a full keyword + AI scan on a single thread. Returns the new scan result.</summary>
        public async Task<ThreadScanResult> ScanThreadAsync(int threadId, int parentId)
        {
            var messages = await _db.ChatLogs
                .Where(m => m.ThreadId == threadId && !m.IsDeleted)
                .OrderBy(m => m.EntryTime)
                .ToListAsync();

            var activeRules = await _flagRuleService.GetParentRulesAsync(parentId);
            activeRules = activeRules.Where(r => r.IsActive).ToList();
            var sensitivity = await _flagRuleService.GetSensitivityAsync(parentId);
            var ruleSetHash = await _flagRuleService.GetRuleSetHashAsync(parentId);

            // Remove any prior scan result for this thread by this parent
            var prior = await _db.ThreadScanResults
                .Where(r => r.ThreadId == threadId && r.ScannedByParentId == parentId && !r.IsDeleted)
                .ToListAsync();
            foreach (var p in prior)
                p.IsDeleted = true;
            await _db.SaveChangesAsync();

            var scanResult = new ThreadScanResult
            {
                ThreadId = threadId,
                ScannedByParentId = parentId,
                ScannedAt = DateTime.UtcNow,
                RuleSetHash = ruleSetHash,
                IsDeleted = false
            };
            _db.ThreadScanResults.Add(scanResult);
            await _db.SaveChangesAsync();

            var flagDetails = new List<ThreadFlagDetail>();

            // Phase 1: keyword scan
            var keywordRules = activeRules.Where(r => r.Type == FlagRuleType.Keyword).ToList();
            flagDetails.AddRange(RunKeywordScan(messages, keywordRules, scanResult.Id));

            // Phase 2: AI topic/tone scan
            var aiRules = activeRules.Where(r => r.Type == FlagRuleType.Topic || r.Type == FlagRuleType.Tone).ToList();
            if (aiRules.Any() && messages.Any())
            {
                var (aiFlags, summary) = await RunAiScanAsync(messages, aiRules, sensitivity, scanResult.Id);
                flagDetails.AddRange(aiFlags);
                scanResult.AiSummary = summary;
            }

            if (flagDetails.Any())
            {
                scanResult.IsFlagged = true;
                foreach (var d in flagDetails)
                    d.IsDeleted = false;
                _db.ThreadFlagDetails.AddRange(flagDetails);
            }

            await _db.SaveChangesAsync();
            return scanResult;
        }

        /// <summary>
        /// Scans all threads visible to this parent. Fires progressCallback(scanned, total) after each thread.
        /// </summary>
        public async Task ScanAllThreadsAsync(int parentId, Func<int, int, Task>? progressCallback = null)
        {
            // Get all children of this parent (or all non-parent threads if linking isn't set up)
            var childIds = await _db.ParentChildren
                .Where(pc => pc.ParentId == parentId)
                .Select(pc => pc.ChildId)
                .ToListAsync();

            List<int> threadIds;
            if (childIds.Any())
            {
                threadIds = await _db.Threads
                    .Where(t => !t.IsDeleted && childIds.Contains(t.UserId))
                    .Select(t => t.Id)
                    .ToListAsync();
            }
            else
            {
                // Fallback: scan all threads not owned by the parent themselves
                threadIds = await _db.Threads
                    .Where(t => !t.IsDeleted && t.UserId != parentId)
                    .Select(t => t.Id)
                    .ToListAsync();
            }

            for (int i = 0; i < threadIds.Count; i++)
            {
                await ScanThreadAsync(threadIds[i], parentId);
                if (progressCallback != null)
                    await progressCallback(i + 1, threadIds.Count);
            }
        }

        /// <summary>Returns the latest (non-deleted) scan result for a thread, including flag details.</summary>
        public async Task<ThreadScanResult?> GetLatestScanResultAsync(int threadId, int parentId) =>
            await _db.ThreadScanResults
                .Include(r => r.FlagDetails)
                .Where(r => r.ThreadId == threadId && r.ScannedByParentId == parentId && !r.IsDeleted)
                .OrderByDescending(r => r.ScannedAt)
                .FirstOrDefaultAsync();

        /// <summary>Returns all flagged threads for the parent, newest scan first.</summary>
        public async Task<List<ThreadScanResult>> GetFlaggedResultsAsync(int parentId) =>
            await _db.ThreadScanResults
                .Include(r => r.Thread)
                .Include(r => r.FlagDetails)
                .Where(r => r.ScannedByParentId == parentId && r.IsFlagged && !r.IsDeleted)
                .OrderByDescending(r => r.ScannedAt)
                .ToListAsync();

        /// <summary>Returns true if the scan result was produced with a different rule set than the current one.</summary>
        public async Task<bool> IsResultStaleAsync(ThreadScanResult result, int parentId)
        {
            var currentHash = await _flagRuleService.GetRuleSetHashAsync(parentId);
            return result.RuleSetHash != currentHash;
        }

        // ── Phase 1: Keyword scan ─────────────────────────────────────────────────

        private static List<ThreadFlagDetail> RunKeywordScan(
            List<ChatLog> messages,
            List<ParentFlagRule> keywordRules,
            int scanResultId)
        {
            var flags = new List<ThreadFlagDetail>();

            foreach (var msg in messages)
            {
                foreach (var rule in keywordRules)
                {
                    // Word-boundary match, case-insensitive
                    var pattern = $@"\b{Regex.Escape(rule.Value)}\b";
                    var match = Regex.Match(msg.Text, pattern, RegexOptions.IgnoreCase);
                    if (!match.Success) continue;

                    // Grab a short excerpt around the match
                    var start = Math.Max(0, match.Index - 30);
                    var length = Math.Min(msg.Text.Length - start, match.Length + 60);
                    var excerpt = msg.Text.Substring(start, length).Trim();

                    flags.Add(new ThreadFlagDetail
                    {
                        ThreadScanResultId = scanResultId,
                        MessageId = msg.Id,
                        RuleType = FlagRuleType.Keyword,
                        RuleValue = rule.Value,
                        MatchedExcerpt = excerpt,
                        Source = FlagSource.Keyword
                    });
                }
            }

            return flags;
        }

        // ── Phase 2: AI scan ──────────────────────────────────────────────────────

        private async Task<(List<ThreadFlagDetail> flags, string? summary)> RunAiScanAsync(
            List<ChatLog> messages,
            List<ParentFlagRule> aiRules,
            ScanSensitivity sensitivity,
            int scanResultId)
        {
            var topicRules = aiRules.Where(r => r.Type == FlagRuleType.Topic).Select(r => r.Value).ToList();
            var toneRules = aiRules.Where(r => r.Type == FlagRuleType.Tone).Select(r => r.Value).ToList();

            var sensitivityInstruction = sensitivity switch
            {
                ScanSensitivity.Low => "SENSITIVITY: Low — Only flag if the conversation explicitly and unambiguously involves the topic or tone. Do not flag tangential or accidental mentions.",
                ScanSensitivity.High => "SENSITIVITY: High — Flag if the conversation touches on the topic or tone in any way, including indirect references or loosely adjacent subjects.",
                _ => "SENSITIVITY: Medium — Flag if the conversation meaningfully involves the topic or tone, even if it is not the main focus of the conversation."
            };

            var conversationText = BuildConversationText(messages);

            var jsonTemplate =
                "{\n" +
                "  \"flagged\": true or false,\n" +
                "  \"topics_found\": [\"exact topic from list\", ...],\n" +
                "  \"tones_found\": [\"exact tone from list\", ...],\n" +
                "  \"summary\": \"2-3 sentence parent-friendly summary. If nothing was flagged, write: No concerning content found.\"\n" +
                "}";

            var prompt =
                "You are a content safety assistant for a parental monitoring application.\n" +
                "Analyze the following child-AI chat conversation for potentially concerning content.\n" +
                "Respond ONLY with valid JSON — no markdown, no explanation, no code fences.\n\n" +
                sensitivityInstruction + "\n\n" +
                "TOPICS TO CHECK (flag if present):\n" +
                string.Join("\n", topicRules.Select(t => $"- {t}")) + "\n\n" +
                "TONES TO CHECK (flag if present):\n" +
                string.Join("\n", toneRules.Select(t => $"- {t}")) + "\n\n" +
                "CONVERSATION:\n" +
                conversationText + "\n\n" +
                "Respond with this exact JSON structure:\n" +
                jsonTemplate;

            var setting = await _ollamaSettingService.GetActiveSettingAsync();
            var url = setting?.ModelUrl ?? DefaultUrl;
            var modelName = setting?.ModelName ?? DefaultModel;

            string rawJson;
            try
            {
                IChatClient client = new OllamaApiClient(new Uri(url), modelName);
                var chatMessages = new List<ChatMessage>
                {
                    new ChatMessage(ChatRole.User, prompt)
                };

                var sb = new StringBuilder();
                await foreach (var chunk in client.GetStreamingResponseAsync(chatMessages))
                    sb.Append(chunk.Text);

                rawJson = sb.ToString().Trim();
            }
            catch
            {
                return (new List<ThreadFlagDetail>(), "AI scan could not be completed. Check that Ollama is running.");
            }

            return ParseAiResponse(rawJson, aiRules, scanResultId);
        }

        private static (List<ThreadFlagDetail> flags, string? summary) ParseAiResponse(
            string rawJson,
            List<ParentFlagRule> aiRules,
            int scanResultId)
        {
            var flags = new List<ThreadFlagDetail>();
            string? summary = null;

            try
            {
                // Strip markdown code fences if the model wrapped the JSON anyway
                var json = rawJson;
                if (json.StartsWith("```"))
                {
                    json = Regex.Replace(json, @"^```[a-z]*\n?", "", RegexOptions.Multiline);
                    json = json.Replace("```", "").Trim();
                }

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                summary = root.TryGetProperty("summary", out var s) ? s.GetString() : null;

                if (root.TryGetProperty("topics_found", out var topics))
                {
                    foreach (var topic in topics.EnumerateArray())
                    {
                        var val = topic.GetString();
                        if (string.IsNullOrEmpty(val)) continue;
                        var matchedRule = aiRules.FirstOrDefault(r =>
                            r.Type == FlagRuleType.Topic &&
                            string.Equals(r.Value, val, StringComparison.OrdinalIgnoreCase));

                        flags.Add(new ThreadFlagDetail
                        {
                            ThreadScanResultId = scanResultId,
                            MessageId = null,
                            RuleType = FlagRuleType.Topic,
                            RuleValue = matchedRule?.Value ?? val,
                            MatchedExcerpt = null,
                            Source = FlagSource.AI
                        });
                    }
                }

                if (root.TryGetProperty("tones_found", out var tones))
                {
                    foreach (var tone in tones.EnumerateArray())
                    {
                        var val = tone.GetString();
                        if (string.IsNullOrEmpty(val)) continue;
                        var matchedRule = aiRules.FirstOrDefault(r =>
                            r.Type == FlagRuleType.Tone &&
                            string.Equals(r.Value, val, StringComparison.OrdinalIgnoreCase));

                        flags.Add(new ThreadFlagDetail
                        {
                            ThreadScanResultId = scanResultId,
                            MessageId = null,
                            RuleType = FlagRuleType.Tone,
                            RuleValue = matchedRule?.Value ?? val,
                            MatchedExcerpt = null,
                            Source = FlagSource.AI
                        });
                    }
                }
            }
            catch
            {
                // JSON parse failed — return no AI flags but preserve any keyword flags already found
            }

            return (flags, summary);
        }

        private static string BuildConversationText(List<ChatLog> messages)
        {
            var sb = new StringBuilder();
            foreach (var msg in messages)
            {
                var speaker = msg.IsReply ? "AI" : "Child";
                sb.AppendLine($"{speaker}: {msg.Text}");
            }
            return sb.ToString();
        }
    }
}
