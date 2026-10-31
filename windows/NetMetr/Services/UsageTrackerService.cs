using System.IO;
using System.Text.Json;
using NetMetr.Models;

namespace NetMetr.Services;

public class UsageTrackerService
{
    private static readonly string AppDataDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "NetMetr");
    private static readonly string UsageFilePath = Path.Combine(AppDataDir, "daily_usage.json");
    private static readonly string SettingsFilePath = Path.Combine(AppDataDir, "settings.json");

    private UsageHistory _history = new();
    private readonly object _lock = new();

    public UsageTrackerService()
    {
        EnsureDirectory();
        LoadUsage();
    }

    private void EnsureDirectory()
    {
        if (!Directory.Exists(AppDataDir))
        {
            Directory.CreateDirectory(AppDataDir);
        }
    }

    private void LoadUsage()
    {
        lock (_lock)
        {
            try
            {
                if (File.Exists(UsageFilePath))
                {
                    var json = File.ReadAllText(UsageFilePath);
                    _history = JsonSerializer.Deserialize<UsageHistory>(json) ?? new UsageHistory();
                }
            }
            catch
            {
                _history = new UsageHistory();
            }
        }
    }

    public void SaveUsage()
    {
        lock (_lock)
        {
            try
            {
                EnsureDirectory();
                var json = JsonSerializer.Serialize(_history, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(UsageFilePath, json);
            }
            catch
            {
                // Silently ignore disk write issues
            }
        }
    }

    public void RecordUsage(long deltaDownload, long deltaUpload)
    {
        if (deltaDownload <= 0 && deltaUpload <= 0) return;

        lock (_lock)
        {
            var today = _history.GetTodayRecord();
            today.DownloadBytes += deltaDownload;
            today.UploadBytes += deltaUpload;
        }
    }

    public DailyUsageRecord GetTodayUsage()
    {
        lock (_lock)
        {
            return _history.GetTodayRecord();
        }
    }

    public List<DailyUsageRecord> GetHistory(int days = 30)
    {
        lock (_lock)
        {
            return _history.Records.Values
                .OrderByDescending(r => r.DateKey)
                .Take(days)
                .ToList();
        }
    }

    public void ResetTodayUsage()
    {
        lock (_lock)
        {
            var today = _history.GetTodayRecord();
            today.DownloadBytes = 0;
            today.UploadBytes = 0;
            SaveUsage();
        }
    }

    public AppSettings LoadSettings()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
        }
        catch
        {
            // fallback
        }
        return new AppSettings();
    }

    public void SaveSettings(AppSettings settings)
    {
        try
        {
            EnsureDirectory();
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFilePath, json);
        }
        catch
        {
            // fallback
        }
    }
}
