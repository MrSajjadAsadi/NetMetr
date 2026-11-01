using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Timers;
using NetMetr.Models;

namespace NetMetr.Services;

public class NetworkAdapterInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public NetworkInterfaceType Type { get; set; }
    public bool IsUp { get; set; }

    public override string ToString() => $"{Name} ({Type})";
}

public class NetworkMonitorService : IDisposable
{
    private readonly System.Timers.Timer _timer;
    private readonly PingService _pingService = new();
    private readonly UsageTrackerService _usageTracker;

    private readonly Dictionary<string, (long Rx, long Tx)> _perAdapterLastStats = new();
    private long _lastBytesReceived;
    private long _lastBytesSent;
    private DateTime _lastSampleTime = DateTime.UtcNow;
    private bool _isFirstSample = true;
    private int _pingCounter = 0;
    private long _lastPingLatency = -1;
    private string _currentActiveAdapterName = "Auto";

    public event Action<NetworkSpeed>? SpeedUpdated;

    public string SelectedAdapterId { get; set; } = "ALL";
    public string PingTargetHost { get; set; } = "8.8.8.8";
    public bool EnablePing { get; set; } = true;

    public NetworkMonitorService(UsageTrackerService usageTracker, int intervalMs = 1000)
    {
        _usageTracker = usageTracker;
        _timer = new System.Timers.Timer(Math.Max(500, intervalMs));
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = true;
    }

    public void Start()
    {
        _isFirstSample = true;
        _perAdapterLastStats.Clear();
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }

    public void SetInterval(int intervalMs)
    {
        _timer.Interval = Math.Max(500, intervalMs);
    }

    public static List<NetworkAdapterInfo> GetAvailableAdapters()
    {
        var list = new List<NetworkAdapterInfo>
        {
            new() { Id = "ALL", Name = "All Active Adapters (Auto)", Description = "Combined throughput across all interfaces", Type = NetworkInterfaceType.Unknown, IsUp = true }
        };

        try
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var ni in interfaces)
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    continue;

                list.Add(new NetworkAdapterInfo
                {
                    Id = ni.Id,
                    Name = ni.Name,
                    Description = ni.Description,
                    Type = ni.NetworkInterfaceType,
                    IsUp = ni.OperationalStatus == OperationalStatus.Up
                });
            }
        }
        catch
        {
            // fallback
        }

        return list;
    }

    private async void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        try
        {
            var now = DateTime.UtcNow;
            double elapsedSeconds = (now - _lastSampleTime).TotalSeconds;
            if (elapsedSeconds <= 0.05) return;

            long currentRx = 0;
            long currentTx = 0;
            string topAdapterName = _currentActiveAdapterName;
            long topDelta = 0;
            string localIp = "-";
            bool hasConnectedInterface = false;

            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var ni in interfaces)
            {
                if (ni.OperationalStatus != OperationalStatus.Up ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    continue;

                if (SelectedAdapterId != "ALL" && ni.Id != SelectedAdapterId)
                    continue;

                try
                {
                    var stats = ni.GetIPStatistics();
                    long niRx = stats.BytesReceived;
                    long niTx = stats.BytesSent;

                    currentRx += niRx;
                    currentTx += niTx;
                    hasConnectedInterface = true;

                    // Calculate per-adapter delta to identify which interface is actually moving data
                    if (_perAdapterLastStats.TryGetValue(ni.Id, out var prevStats))
                    {
                        long dRx = Math.Max(0, niRx - prevStats.Rx);
                        long dTx = Math.Max(0, niTx - prevStats.Tx);
                        long dTotal = dRx + dTx;

                        if (dTotal > topDelta)
                        {
                            topDelta = dTotal;
                            topAdapterName = ni.Name;
                        }
                    }

                    _perAdapterLastStats[ni.Id] = (niRx, niTx);

                    if (SelectedAdapterId != "ALL")
                    {
                        topAdapterName = ni.Name;
                    }

                    if (localIp == "-" && stats.BytesReceived > 0)
                    {
                        var ipProps = ni.GetIPProperties();
                        foreach (var unicast in ipProps.UnicastAddresses)
                        {
                            if (unicast.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                localIp = unicast.Address.ToString();
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    // interface might have disconnected or stats unsupported
                }
            }

            if (_isFirstSample)
            {
                _lastBytesReceived = currentRx;
                _lastBytesSent = currentTx;
                _lastSampleTime = now;
                _isFirstSample = false;
                return;
            }

            long deltaRx = Math.Max(0, currentRx - _lastBytesReceived);
            long deltaTx = Math.Max(0, currentTx - _lastBytesSent);

            _lastBytesReceived = currentRx;
            _lastBytesSent = currentTx;
            _lastSampleTime = now;

            double speedRx = deltaRx / elapsedSeconds;
            double speedTx = deltaTx / elapsedSeconds;

            if (topDelta > 0)
            {
                _currentActiveAdapterName = topAdapterName;
            }
            else if (SelectedAdapterId == "ALL" && _currentActiveAdapterName == "Auto")
            {
                _currentActiveAdapterName = topAdapterName;
            }

            // Track daily usage
            _usageTracker.RecordUsage(deltaRx, deltaTx);

            // Periodic ping (every ~2 seconds)
            _pingCounter++;
            if (EnablePing && (_pingCounter % 2 == 0 || _lastPingLatency == -1))
            {
                _lastPingLatency = await _pingService.PingHostAsync(PingTargetHost);
            }

            var speed = new NetworkSpeed
            {
                DownloadBytesPerSec = speedRx,
                UploadBytesPerSec = speedTx,
                LatencyMs = EnablePing ? _lastPingLatency : -1,
                IsConnected = hasConnectedInterface || NetworkInterface.GetIsNetworkAvailable(),
                ActiveInterfaceName = SelectedAdapterId == "ALL" ? _currentActiveAdapterName : topAdapterName,
                InterfaceType = "Active",
                LocalIpAddress = localIp
            };

            SpeedUpdated?.Invoke(speed);
        }
        catch
        {
            // Safeguard against OS network exceptions during sleep/wake
        }
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
    }
}
