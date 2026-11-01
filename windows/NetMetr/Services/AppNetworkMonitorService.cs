using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using NetMetr.Models;

namespace NetMetr.Services;

public class AppNetworkMonitorService : IDisposable
{
    private readonly System.Timers.Timer _timer;
    private readonly Dictionary<int, (ulong ReadBytes, ulong WriteBytes, DateTime Time)> _lastCounters = new();
    private readonly Dictionary<int, (long TotalRx, long TotalTx)> _cumulativeUsage = new();
    private readonly Dictionary<int, (string Name, string Path, BitmapSource? Icon)> _processMetaCache = new();

    public event Action<List<AppNetworkInfo>>? AppsUpdated;

    public AppNetworkMonitorService(int intervalMs = 1500)
    {
        _timer = new System.Timers.Timer(Math.Max(1000, intervalMs));
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = true;
    }

    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        try
        {
            var activePids = GetNetworkActivePids();
            var blockedNames = FirewallManager.GetBlockedAppNames();
            var now = DateTime.UtcNow;
            var resultList = new List<AppNetworkInfo>();

            foreach (var kvp in activePids)
            {
                int pid = kvp.Key;
                int socketCount = kvp.Value;

                if (pid <= 4) continue; // Skip System and Idle

                var (name, path, icon) = GetProcessMetadata(pid);
                if (string.IsNullOrEmpty(name)) continue;

                double downSpeed = 0;
                double upSpeed = 0;

                // Try reading I/O counters
                if (GetProcessIoCountersByPid(pid, out ulong readBytes, out ulong writeBytes))
                {
                    if (_lastCounters.TryGetValue(pid, out var last))
                    {
                        double elapsed = (now - last.Time).TotalSeconds;
                        if (elapsed > 0.1)
                        {
                            long deltaRead = (long)readBytes - (long)last.ReadBytes;
                            long deltaWrite = (long)writeBytes - (long)last.WriteBytes;

                            if (deltaRead > 0)
                            {
                                downSpeed = deltaRead / elapsed;
                                if (!_cumulativeUsage.ContainsKey(pid)) _cumulativeUsage[pid] = (0, 0);
                                _cumulativeUsage[pid] = (_cumulativeUsage[pid].TotalRx + deltaRead, _cumulativeUsage[pid].TotalTx);
                            }

                            if (deltaWrite > 0)
                            {
                                upSpeed = deltaWrite / elapsed;
                                if (!_cumulativeUsage.ContainsKey(pid)) _cumulativeUsage[pid] = (0, 0);
                                _cumulativeUsage[pid] = (_cumulativeUsage[pid].TotalRx, _cumulativeUsage[pid].TotalTx + deltaWrite);
                            }
                        }
                    }

                    _lastCounters[pid] = (readBytes, writeBytes, now);
                }

                _cumulativeUsage.TryGetValue(pid, out var cumulative);
                string cleanName = name.Replace(".exe", "", StringComparison.OrdinalIgnoreCase).Trim();
                bool isBlocked = blockedNames.Contains(cleanName);

                resultList.Add(new AppNetworkInfo
                {
                    ProcessId = pid,
                    ProcessName = name,
                    ExecutablePath = path,
                    AppIcon = icon,
                    ActiveConnectionsCount = socketCount,
                    DownloadSpeedBytesPerSec = downSpeed,
                    UploadSpeedBytesPerSec = upSpeed,
                    TotalDownloadedBytes = cumulative.TotalRx,
                    TotalUploadedBytes = cumulative.TotalTx,
                    IsBlocked = isBlocked
                });
            }

            // Cleanup dead PIDs from cache
            var deadPids = _lastCounters.Keys.Where(p => !activePids.ContainsKey(p)).ToList();
            foreach (var dead in deadPids)
            {
                _lastCounters.Remove(dead);
            }

            AppsUpdated?.Invoke(resultList.OrderByDescending(a => a.DownloadSpeedBytesPerSec + a.UploadSpeedBytesPerSec)
                                         .ThenByDescending(a => a.TotalDownloadedBytes + a.TotalUploadedBytes)
                                         .ToList());
        }
        catch
        {
            // Safeguard against OS process exceptions
        }
    }

    private (string Name, string Path, BitmapSource? Icon) GetProcessMetadata(int pid)
    {
        if (_processMetaCache.TryGetValue(pid, out var cached))
            return cached;

        string name = string.Empty;
        string path = string.Empty;
        BitmapSource? icon = null;

        try
        {
            using var proc = Process.GetProcessById(pid);
            name = proc.ProcessName;

            try
            {
                path = proc.MainModule?.FileName ?? string.Empty;
            }
            catch
            {
                // Access denied for system processes or different architectures
            }

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                try
                {
                    using var ico = System.Drawing.Icon.ExtractAssociatedIcon(path);
                    if (ico != null)
                    {
                        icon = Imaging.CreateBitmapSourceFromHIcon(
                            ico.Handle,
                            System.Windows.Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
                        icon.Freeze();
                    }
                }
                catch
                {
                    // Fallback if icon extraction fails
                }
            }
        }
        catch
        {
            return (string.Empty, string.Empty, null);
        }

        var entry = (name, path, icon);
        _processMetaCache[pid] = entry;
        return entry;
    }

    private Dictionary<int, int> GetNetworkActivePids()
    {
        var pidCounts = new Dictionary<int, int>();

        // IPv4 TCP
        CollectTcpPids(2, pidCounts);
        // IPv6 TCP
        CollectTcpPids(23, pidCounts);
        // IPv4 UDP
        CollectUdpPids(2, pidCounts);
        // IPv6 UDP
        CollectUdpPids(23, pidCounts);

        return pidCounts;
    }

    private void CollectTcpPids(int ipVersion, Dictionary<int, int> pidCounts)
    {
        int buffSize = 0;
        NativeMethods.GetExtendedTcpTable(IntPtr.Zero, ref buffSize, false, ipVersion, 5, 0);
        if (buffSize <= 0) return;

        IntPtr pTable = Marshal.AllocHGlobal(buffSize);
        try
        {
            uint ret = NativeMethods.GetExtendedTcpTable(pTable, ref buffSize, false, ipVersion, 5, 0);
            if (ret != 0) return;

            int numEntries = Marshal.ReadInt32(pTable);
            IntPtr rowPtr = IntPtr.Add(pTable, 4);

            int rowSize = ipVersion == 2 ? 24 : 56; // MIB_TCPROW_OWNER_PID vs MIB_TCP6ROW_OWNER_PID
            int pidOffset = ipVersion == 2 ? 20 : 52;

            for (int i = 0; i < numEntries; i++)
            {
                int pid = Marshal.ReadInt32(rowPtr, pidOffset);
                if (pid > 0)
                {
                    pidCounts[pid] = pidCounts.GetValueOrDefault(pid) + 1;
                }
                rowPtr = IntPtr.Add(rowPtr, rowSize);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(pTable);
        }
    }

    private void CollectUdpPids(int ipVersion, Dictionary<int, int> pidCounts)
    {
        int buffSize = 0;
        NativeMethods.GetExtendedUdpTable(IntPtr.Zero, ref buffSize, false, ipVersion, 1, 0);
        if (buffSize <= 0) return;

        IntPtr pTable = Marshal.AllocHGlobal(buffSize);
        try
        {
            uint ret = NativeMethods.GetExtendedUdpTable(pTable, ref buffSize, false, ipVersion, 1, 0);
            if (ret != 0) return;

            int numEntries = Marshal.ReadInt32(pTable);
            IntPtr rowPtr = IntPtr.Add(pTable, 4);

            int rowSize = ipVersion == 2 ? 12 : 28; // MIB_UDPROW_OWNER_PID vs MIB_UDP6ROW_OWNER_PID
            int pidOffset = ipVersion == 2 ? 8 : 24;

            for (int i = 0; i < numEntries; i++)
            {
                int pid = Marshal.ReadInt32(rowPtr, pidOffset);
                if (pid > 0)
                {
                    pidCounts[pid] = pidCounts.GetValueOrDefault(pid) + 1;
                }
                rowPtr = IntPtr.Add(rowPtr, rowSize);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(pTable);
        }
    }

    private bool GetProcessIoCountersByPid(int pid, out ulong readBytes, out ulong writeBytes)
    {
        readBytes = 0;
        writeBytes = 0;

        IntPtr hProcess = NativeMethods.OpenProcess(0x0400 | 0x0010, false, pid); // PROCESS_QUERY_INFORMATION | PROCESS_VM_READ
        if (hProcess == IntPtr.Zero) return false;

        try
        {
            if (NativeMethods.GetProcessIoCounters(hProcess, out var counters))
            {
                readBytes = counters.ReadTransferCount;
                writeBytes = counters.WriteTransferCount;
                return true;
            }
        }
        finally
        {
            NativeMethods.CloseHandle(hProcess);
        }

        return false;
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
    }

    private static class NativeMethods
    {
        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern uint GetExtendedTcpTable(IntPtr pTcpTable, ref int pdwOutBufLen, bool sort, int ipVersion, int tblClass, uint reserved);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern uint GetExtendedUdpTable(IntPtr pUdpTable, ref int pdwOutBufLen, bool sort, int ipVersion, int tblClass, uint reserved);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetProcessIoCounters(IntPtr hProcess, out IO_COUNTERS counters);

        [StructLayout(LayoutKind.Sequential)]
        public struct IO_COUNTERS
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }
    }
}
