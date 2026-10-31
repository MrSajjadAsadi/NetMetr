namespace NetMetr.Models;

public enum SpeedUnit
{
    BytesPerSecond,  // B/s, KB/s, MB/s
    BitsPerSecond    // bps, Kbps, Mbps
}

public class AppSettings
{
    public double WindowLeft { get; set; } = -1;
    public double WindowTop { get; set; } = -1;
    public bool AlwaysOnTop { get; set; } = true;
    public bool IsLocked { get; set; } = false;
    public bool IsCompactMode { get; set; } = false;
    public double WindowOpacity { get; set; } = 0.92;
    public SpeedUnit Unit { get; set; } = SpeedUnit.BytesPerSecond;
    public int RefreshIntervalMs { get; set; } = 1000;
    public string SelectedAdapterId { get; set; } = "ALL";
    public string PingTargetHost { get; set; } = "8.8.8.8";
    public bool EnablePing { get; set; } = true;
    public bool StartWithWindows { get; set; } = false;
    public bool SnapToEdges { get; set; } = true;
}
