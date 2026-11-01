using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace NetMetr.Services;

public class PingService
{
    private readonly Ping _ping = new();
    private bool _isPinging = false;

    public async Task<long> PingHostAsync(string host, int timeoutMs = 1500)
    {
        if (_isPinging) return -1;
        _isPinging = true;

        try
        {
            if (string.IsNullOrWhiteSpace(host))
                host = "8.8.8.8";

            // 1. Try standard ICMP Ping first
            try
            {
                var reply = await _ping.SendPingAsync(host, Math.Min(timeoutMs, 800));
                if (reply.Status == IPStatus.Success)
                {
                    return reply.RoundtripTime;
                }
            }
            catch
            {
                // ICMP blocked or unsupported (e.g. on PdaNet+, Android VPN tethering, firewalls).
                // Proceed to TCP connect ping fallback.
            }

            // 2. Fallback: Fast TCP Connect Ping (DNS port 53 or 443)
            // Works 100% through PdaNet+, mobile VPNs, proxies, and tethering
            string targetHost = host;
            int port = 53; // Default to DNS port (open on 8.8.8.8, 1.1.1.1, etc.)

            if (targetHost.Contains(':'))
            {
                var parts = targetHost.Split(':');
                targetHost = parts[0];
                if (int.TryParse(parts[1], out int customPort))
                    port = customPort;
            }

            var sw = Stopwatch.StartNew();
            using var client = new TcpClient();
            using var cts = new CancellationTokenSource(timeoutMs);

            await client.ConnectAsync(targetHost, port, cts.Token);
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
        catch
        {
            return -1;
        }
        finally
        {
            _isPinging = false;
        }
    }
}
