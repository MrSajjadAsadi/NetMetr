namespace NetMetr.Models;

public class NetworkSpeed
{
    public double DownloadBytesPerSec { get; set; }
    public double UploadBytesPerSec { get; set; }

    public double DownloadBitsPerSec => DownloadBytesPerSec * 8;
    public double UploadBitsPerSec => UploadBytesPerSec * 8;

    public long LatencyMs { get; set; } = -1;
    public bool IsConnected { get; set; }
    public string ActiveInterfaceName { get; set; } = "Auto";
    public string InterfaceType { get; set; } = "Unknown";
    public string LocalIpAddress { get; set; } = "-";
}
