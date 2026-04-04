using FamilyAI.Domain.Data;
using FamilyAI.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FamilyAI.Infrastructure.Services
{
    public class PromptSettingService
    {
        private readonly MyDbContext _db;

        public PromptSettingService(MyDbContext db)
        {
            _db = db;
        }

        // ── Templates ────────────────────────────────────────────────────────────

        public async Task<List<PromptTemplate>> GetAllTemplatesAsync()
        {
            return await _db.PromptTemplates
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<PromptTemplate> CreateTemplateAsync(string name, string content, bool isDefault = false)
        {
            if (isDefault)
                await ClearDefaultFlagAsync();

            var template = new PromptTemplate
            {
                Name = name,
                Content = content,
                IsDefault = isDefault,
                IsDeleted = false
            };
            _db.PromptTemplates.Add(template);
            await _db.SaveChangesAsync();
            return template;
        }

        public async Task<bool> UpdateTemplateAsync(int id, string name, string content, bool isDefault)
        {
            var template = await _db.PromptTemplates.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (template == null) return false;

            if (isDefault && !template.IsDefault)
                await ClearDefaultFlagAsync();

            template.Name = name;
            template.Content = content;
            template.IsDefault = isDefault;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTemplateAsync(int id)
        {
            // Block if any child has this template assigned
            bool inUse = await _db.UserSettings.AnyAsync(us => us.PromptTemplateId == id && !us.IsDeleted);
            if (inUse) return false;

            var template = await _db.PromptTemplates.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (template == null) return false;

            // Don't delete if it's the only template
            int count = await _db.PromptTemplates.CountAsync(p => !p.IsDeleted);
            if (count <= 1) return false;

            template.IsDeleted = true;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetUsageCountAsync(int templateId)
        {
            return await _db.UserSettings.CountAsync(us => us.PromptTemplateId == templateId && !us.IsDeleted);
        }

        // ── Per-user setting ─────────────────────────────────────────────────────

        /// <summary>Returns the active prompt template for a child user.
        /// Falls back to the default template if no setting exists.</summary>
        public async Task<PromptTemplate?> GetActiveTemplateForUserAsync(int userId)
        {
            var setting = await _db.UserSettings
                .AsNoTracking()
                .Include(us => us.PromptTemplate)
                .FirstOrDefaultAsync(us => us.UserId == userId && !us.IsDeleted);

            if (setting != null)
                return setting.PromptTemplate;

            // Fall back to the default template
            return await _db.PromptTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IsDefault && !p.IsDeleted)
                ?? await _db.PromptTemplates
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => !p.IsDeleted);
        }

        public async Task SetActiveTemplateForUserAsync(int userId, int templateId)
        {
            var existing = await _db.UserSettings
                .FirstOrDefaultAsync(us => us.UserId == userId);

            if (existing != null)
            {
                existing.PromptTemplateId = templateId;
                existing.IsDeleted = false;
            }
            else
            {
                _db.UserSettings.Add(new UserSetting
                {
                    UserId = userId,
                    PromptTemplateId = templateId,
                    IsDeleted = false
                });
            }
            await _db.SaveChangesAsync();
        }

        public async Task<int> GetActiveTemplateIdForUserAsync(int userId)
        {
            var setting = await _db.UserSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.UserId == userId && !us.IsDeleted);

            if (setting != null) return setting.PromptTemplateId;

            var def = await _db.PromptTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IsDefault && !p.IsDeleted);

            return def?.Id ?? 0;
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private async Task ClearDefaultFlagAsync()
        {
            var current = await _db.PromptTemplates.Where(p => p.IsDefault && !p.IsDeleted).ToListAsync();
            foreach (var p in current) p.IsDefault = false;
        }
    }
}
