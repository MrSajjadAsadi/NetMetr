using System.Net.NetworkInformation;

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

            var reply = await _ping.SendPingAsync(host, timeoutMs);
            if (reply.Status == IPStatus.Success)
            {
                return reply.RoundtripTime;
            }
            return -1;
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
