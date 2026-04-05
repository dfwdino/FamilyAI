using FamilyAI.Domain.Data;
using FamilyAI.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FamilyAI.Infrastructure.Services;

public class OllamaSettingService
{
    private readonly MyDbContext _db;
    private readonly IHttpClientFactory _httpClientFactory;

    public OllamaSettingService(MyDbContext db, IHttpClientFactory httpClientFactory)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<OllamaSetting>> GetAllSettingsAsync() =>
        await _db.OllamaSettings
            .Where(s => !s.IsDeleted)
            .OrderByDescending(s => s.IsActive)
            .ThenBy(s => s.Name)
            .ToListAsync();

    /// <summary>Returns the active setting, or null if none exist (caller uses hardcoded defaults).</summary>
    public async Task<OllamaSetting?> GetActiveSettingAsync() =>
        await _db.OllamaSettings
            .Where(s => !s.IsDeleted && s.IsActive)
            .FirstOrDefaultAsync();

    /// <summary>Queries the Ollama /api/tags endpoint and returns the available model names.</summary>
    public async Task<List<string>> GetAvailableModelsAsync(string url)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            var baseUrl = url.TrimEnd('/');
            var response = await client.GetStringAsync($"{baseUrl}/api/tags");
            using var doc = JsonDocument.Parse(response);
            var models = doc.RootElement.GetProperty("models");
            return models.EnumerateArray()
                .Select(m => m.GetProperty("name").GetString() ?? string.Empty)
                .Where(n => !string.IsNullOrEmpty(n))
                .OrderBy(n => n)
                .ToList();
        }
        catch
        {
            return new List<string>();
        }
    }

    public async Task<OllamaSetting> CreateSettingAsync(string name, string modelUrl, string modelName, bool isActive)
    {
        if (isActive)
            await ClearActiveAsync();

        var setting = new OllamaSetting
        {
            Name = name,
            ModelUrl = modelUrl,
            ModelName = modelName,
            IsActive = isActive,
            IsDeleted = false
        };
        _db.OllamaSettings.Add(setting);
        await _db.SaveChangesAsync();
        return setting;
    }

    public async Task UpdateSettingAsync(int id, string name, string modelUrl, string modelName, bool isActive)
    {
        var setting = await _db.OllamaSettings.FindAsync(id)
            ?? throw new InvalidOperationException("Setting not found.");

        if (isActive && !setting.IsActive)
            await ClearActiveAsync();

        setting.Name = name;
        setting.ModelUrl = modelUrl;
        setting.ModelName = modelName;
        setting.IsActive = isActive;
        await _db.SaveChangesAsync();
    }

    /// <summary>Soft-deletes a setting. Returns false if it's active or the last remaining one.</summary>
    public async Task<bool> DeleteSettingAsync(int id)
    {
        var setting = await _db.OllamaSettings.FindAsync(id);
        if (setting == null || setting.IsDeleted || setting.IsActive)
            return false;

        var count = await _db.OllamaSettings.CountAsync(s => !s.IsDeleted);
        if (count <= 1)
            return false;

        setting.IsDeleted = true;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task SetActiveAsync(int id)
    {
        await ClearActiveAsync();
        var setting = await _db.OllamaSettings.FindAsync(id)
            ?? throw new InvalidOperationException("Setting not found.");
        setting.IsActive = true;
        await _db.SaveChangesAsync();
    }

    private async Task ClearActiveAsync()
    {
        var actives = await _db.OllamaSettings.Where(s => !s.IsDeleted && s.IsActive).ToListAsync();
        foreach (var s in actives)
            s.IsActive = false;
        await _db.SaveChangesAsync();
    }
}
