using FamilyAI.Domain.Data;
using FamilyAI.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace FamilyAI.Infrastructure.Services
{
    public class FlagRuleService
    {
        private readonly MyDbContext _db;

        public FlagRuleService(MyDbContext db)
        {
            _db = db;
        }

        // ── Global rules ─────────────────────────────────────────────────────────

        public async Task<List<GlobalFlagRule>> GetGlobalRulesAsync() =>
            await _db.GlobalFlagRules
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.Type)
                .ThenBy(r => r.Value)
                .ToListAsync();

        // ── Parent rules ─────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the parent's active rule set. On first call, seeds from global defaults automatically.
        /// </summary>
        public async Task<List<ParentFlagRule>> GetParentRulesAsync(int parentId)
        {
            var hasAny = await _db.ParentFlagRules.AnyAsync(r => r.ParentUserId == parentId && !r.IsDeleted);
            if (!hasAny)
                await SeedFromGlobalAsync(parentId);

            return await _db.ParentFlagRules
                .Where(r => r.ParentUserId == parentId && !r.IsDeleted)
                .OrderBy(r => r.Type)
                .ThenBy(r => r.Value)
                .ToListAsync();
        }

        public async Task AddParentRuleAsync(int parentId, FlagRuleType type, string value, string description)
        {
            _db.ParentFlagRules.Add(new ParentFlagRule
            {
                ParentUserId = parentId,
                Type = type,
                Value = value.Trim(),
                Description = description.Trim(),
                IsActive = true,
                IsDeleted = false
            });
            await _db.SaveChangesAsync();
        }

        public async Task ToggleParentRuleAsync(int ruleId, bool isActive)
        {
            var rule = await _db.ParentFlagRules.FindAsync(ruleId);
            if (rule == null || rule.IsDeleted) return;
            rule.IsActive = isActive;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteParentRuleAsync(int ruleId)
        {
            var rule = await _db.ParentFlagRules.FindAsync(ruleId);
            if (rule == null) return;
            rule.IsDeleted = true;
            await _db.SaveChangesAsync();
        }

        /// <summary>Wipes all parent rules and re-seeds from global defaults.</summary>
        public async Task ResetToDefaultsAsync(int parentId)
        {
            var existing = await _db.ParentFlagRules
                .Where(r => r.ParentUserId == parentId)
                .ToListAsync();
            _db.ParentFlagRules.RemoveRange(existing);
            await _db.SaveChangesAsync();
            await SeedFromGlobalAsync(parentId);
        }

        // ── Sensitivity setting ───────────────────────────────────────────────────

        public async Task<ScanSensitivity> GetSensitivityAsync(int parentId)
        {
            var setting = await _db.ParentScanSettings
                .FirstOrDefaultAsync(s => s.ParentUserId == parentId && !s.IsDeleted);
            return setting?.Sensitivity ?? ScanSensitivity.Medium;
        }

        public async Task SetSensitivityAsync(int parentId, ScanSensitivity sensitivity)
        {
            var setting = await _db.ParentScanSettings
                .FirstOrDefaultAsync(s => s.ParentUserId == parentId && !s.IsDeleted);

            if (setting == null)
            {
                _db.ParentScanSettings.Add(new ParentScanSetting
                {
                    ParentUserId = parentId,
                    Sensitivity = sensitivity,
                    IsDeleted = false
                });
            }
            else
            {
                setting.Sensitivity = sensitivity;
            }
            await _db.SaveChangesAsync();
        }

        // ── Rule set hash (stale detection) ───────────────────────────────────────

        /// <summary>
        /// Returns a deterministic hash of the parent's current active rules.
        /// If the hash stored on a scan result differs, the result is stale.
        /// </summary>
        public async Task<string> GetRuleSetHashAsync(int parentId)
        {
            var rules = await _db.ParentFlagRules
                .Where(r => r.ParentUserId == parentId && !r.IsDeleted && r.IsActive)
                .OrderBy(r => r.Type)
                .ThenBy(r => r.Value)
                .Select(r => $"{(int)r.Type}:{r.Value.ToLowerInvariant()}")
                .ToListAsync();

            var sensitivity = await GetSensitivityAsync(parentId);
            rules.Add($"sensitivity:{(int)sensitivity}");

            var combined = string.Join("|", rules);
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(combined));
            return Convert.ToHexString(bytes)[..16];
        }

        // ── Private helpers ───────────────────────────────────────────────────────

        private async Task SeedFromGlobalAsync(int parentId)
        {
            var globals = await _db.GlobalFlagRules
                .Where(r => !r.IsDeleted)
                .ToListAsync();

            foreach (var g in globals)
            {
                _db.ParentFlagRules.Add(new ParentFlagRule
                {
                    ParentUserId = parentId,
                    Type = g.Type,
                    Value = g.Value,
                    Description = g.Description,
                    GlobalRuleId = g.Id,
                    IsActive = true,
                    IsDeleted = false
                });
            }
            await _db.SaveChangesAsync();
        }
    }
}
