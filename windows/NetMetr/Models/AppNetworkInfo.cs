using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetMetr.Models;

public class AppNetworkInfo : INotifyPropertyChanged
{
    private double _downloadSpeed;
    private double _uploadSpeed;
    private long _totalDownloaded;
    private long _totalUploaded;
    private int _connectionsCount;
    private bool _isBlocked;

    public int ProcessId { get; set; }
    public string ProcessName { get; set; } = string.Empty;
    public string ExecutablePath { get; set; } = string.Empty;
    public BitmapSource? AppIcon { get; set; }

    public double DownloadSpeedBytesPerSec
    {
        get => _downloadSpeed;
        set { _downloadSpeed = value; OnPropertyChanged(); OnPropertyChanged(nameof(DownloadSpeedFormatted)); }
    }

    public double UploadSpeedBytesPerSec
    {
        get => _uploadSpeed;
        set { _uploadSpeed = value; OnPropertyChanged(); OnPropertyChanged(nameof(UploadSpeedFormatted)); }
    }

    public long TotalDownloadedBytes
    {
        get => _totalDownloaded;
        set { _totalDownloaded = value; OnPropertyChanged(); OnPropertyChanged(nameof(TotalUsageFormatted)); }
    }

    public long TotalUploadedBytes
    {
        get => _totalUploaded;
        set { _totalUploaded = value; OnPropertyChanged(); OnPropertyChanged(nameof(TotalUsageFormatted)); }
    }

    public int ActiveConnectionsCount
    {
        get => _connectionsCount;
        set { _connectionsCount = value; OnPropertyChanged(); }
    }

    public bool IsBlocked
    {
        get => _isBlocked;
        set { _isBlocked = value; OnPropertyChanged(); OnPropertyChanged(nameof(StatusText)); OnPropertyChanged(nameof(BlockButtonText)); }
    }

    public string DownloadSpeedFormatted => Helpers.Formatter.FormatSpeed(DownloadSpeedBytesPerSec, SpeedUnit.BytesPerSecond);
    public string UploadSpeedFormatted => Helpers.Formatter.FormatSpeed(UploadSpeedBytesPerSec, SpeedUnit.BytesPerSecond);
    public string TotalUsageFormatted => Helpers.Formatter.FormatDataSize(TotalDownloadedBytes + TotalUploadedBytes);
    public string StatusText => IsBlocked ? Helpers.LocalizationManager.Get("StatusBlocked") : Helpers.LocalizationManager.Get("StatusActive");
    public string BlockButtonText => IsBlocked ? Helpers.LocalizationManager.Get("BtnUnblock") : Helpers.LocalizationManager.Get("BtnBlock");

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
