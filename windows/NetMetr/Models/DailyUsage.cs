using System.Text.Json.Serialization;

namespace NetMetr.Models;

public class DailyUsageRecord
{
    public string DateKey { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
    public long DownloadBytes { get; set; }
    public long UploadBytes { get; set; }
    public long TotalBytes => DownloadBytes + UploadBytes;
}

public class UsageHistory
{
    public Dictionary<string, DailyUsageRecord> Records { get; set; } = new();

    public DailyUsageRecord GetTodayRecord()
    {
        var key = DateTime.Today.ToString("yyyy-MM-dd");
        if (!Records.TryGetValue(key, out var record))
        {
            record = new DailyUsageRecord { DateKey = key };
            Records[key] = record;
        }
        return record;
    }
}
