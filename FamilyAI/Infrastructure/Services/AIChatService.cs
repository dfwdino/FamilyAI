using FamilyAI.Domain.Models;
using FamilyAI.Infrastructure.configuration;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace FamilyAI.Infrastructure.Services
{
    public class AIChatService
    {
        private readonly OllamaSettingService _ollamaSettingService;
        private IChatClient? _chatClient;
        private List<ChatMessage> _chatHistory;

        // Fallback defaults if no DB setting exists yet
        private const string DefaultUrl = "http://localhost:11434/";
        private const string DefaultModel = "llama3.2-vision";

        public AIChatService(OllamaSettingService ollamaSettingService)
        {
            _ollamaSettingService = ollamaSettingService;
            _chatHistory = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, PromptTemplates.Educational)
            };
        }

        /// <summary>
        /// Loads the active Ollama config from DB, sets the system prompt, and loads existing history.
        /// Must be awaited before calling ChatWithAIAsync.
        /// </summary>
        public async Task InitializeAsync(string systemPrompt, List<ChatLog> existingLogs)
        {
            var setting = await _ollamaSettingService.GetActiveSettingAsync();
            var url = setting?.ModelUrl ?? DefaultUrl;
            var modelName = setting?.ModelName ?? DefaultModel;

            _chatClient = new OllamaApiClient(new Uri(url), modelName);

            _chatHistory = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, systemPrompt)
            };

            foreach (var log in existingLogs)
            {
                var role = log.IsReply ? ChatRole.Assistant : ChatRole.User;
                _chatHistory.Add(new ChatMessage(role, log.Text));
            }
        }

        public async Task<string> ChatWithAIAsync(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return string.Empty;

            // Fallback client in case InitializeAsync was never called
            _chatClient ??= new OllamaApiClient(new Uri(DefaultUrl), DefaultModel);

            _chatHistory.Add(new ChatMessage(ChatRole.User, userInput));

            var responseBuilder = new System.Text.StringBuilder();

            await foreach (var update in _chatClient.GetStreamingResponseAsync(_chatHistory))
            {
                responseBuilder.Append(update.Text);
            }

            var responseText = responseBuilder.ToString();
            _chatHistory.Add(new ChatMessage(ChatRole.Assistant, responseText));

            return responseText;
        }
    }
}
