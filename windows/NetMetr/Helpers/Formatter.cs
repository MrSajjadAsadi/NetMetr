using NetMetr.Models;

namespace NetMetr.Helpers;

public static class Formatter
{
    public static string FormatSpeed(double bytesPerSec, SpeedUnit unit)
    {
        if (unit == SpeedUnit.BitsPerSecond)
        {
            double bits = bytesPerSec * 8;
            if (bits >= 1_000_000_000)
                return $"{bits / 1_000_000_000.0:F2} Gbps";
            if (bits >= 1_000_000)
                return $"{bits / 1_000_000.0:F2} Mbps";
            if (bits >= 1_000)
                return $"{bits / 1_000.0:F1} Kbps";
            return $"{bits:F0} bps";
        }
        else
        {
            if (bytesPerSec >= 1_073_741_824)
                return $"{bytesPerSec / 1_073_741_824.0:F2} GB/s";
            if (bytesPerSec >= 1_048_576)
                return $"{bytesPerSec / 1_048_576.0:F2} MB/s";
            if (bytesPerSec >= 1024)
                return $"{bytesPerSec / 1024.0:F1} KB/s";
            return $"{bytesPerSec:F0} B/s";
        }
    }

    public static string FormatDataSize(long bytes)
    {
        double b = bytes;
        if (b >= 1_073_741_824 * 1024.0)
            return $"{b / (1_073_741_824 * 1024.0):F2} TB";
        if (b >= 1_073_741_824)
            return $"{b / 1_073_741_824.0:F2} GB";
        if (b >= 1_048_576)
            return $"{b / 1_048_576.0:F1} MB";
        if (b >= 1024)
            return $"{b / 1024.0:F0} KB";
        return $"{b:F0} B";
    }

    public static string FormatLatency(long latencyMs, bool isConnected = true)
    {
        if (!isConnected)
            return LocalizationManager.Get("Offline");

        if (latencyMs < 0)
            return "--" + LocalizationManager.Get("Milliseconds");

        return $"{latencyMs}{LocalizationManager.Get("Milliseconds")}";
    }
}
