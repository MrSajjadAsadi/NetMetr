using System.Diagnostics;
using System.IO;

namespace NetMetr.Services;

public static class FirewallManager
{
    private const string RulePrefix = "NetMetr_Block_";

    public static HashSet<string> GetBlockedAppNames()
    {
        var blocked = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            var psi = new ProcessStartInfo("netsh", "advfirewall firewall show rule name=all")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using var proc = Process.Start(psi);
            if (proc == null) return blocked;

            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.Contains("Rule Name:", StringComparison.OrdinalIgnoreCase) &&
                    line.Contains(RulePrefix, StringComparison.OrdinalIgnoreCase))
                {
                    int idx = line.IndexOf(RulePrefix, StringComparison.OrdinalIgnoreCase);
                    if (idx >= 0)
                    {
                        string name = line.Substring(idx + RulePrefix.Length).Trim();
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            blocked.Add(name);
                        }
                    }
                }
            }
        }
        catch
        {
            // fallback if firewall query fails
        }

        return blocked;
    }

    public static bool IsAppBlocked(string appName)
    {
        if (string.IsNullOrWhiteSpace(appName)) return false;
        string cleanName = CleanRuleName(appName);
        return GetBlockedAppNames().Contains(cleanName);
    }

    public static bool BlockApp(string exePath, string appName)
    {
        if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath)) return false;

        string cleanName = CleanRuleName(appName);
        string ruleName = $"{RulePrefix}{cleanName}";

        // Delete any existing rule first
        UnblockApp(appName);

        // Add Outbound block rule
        string outArgs = $"advfirewall firewall add rule name=\"{ruleName}\" dir=out action=block program=\"{exePath}\" enable=yes";
        // Add Inbound block rule
        string inArgs = $"advfirewall firewall add rule name=\"{ruleName}\" dir=in action=block program=\"{exePath}\" enable=yes";

        bool outSuccess = RunNetsh(outArgs);
        bool inSuccess = RunNetsh(inArgs);

        return outSuccess && inSuccess;
    }

    public static bool UnblockApp(string appName)
    {
        if (string.IsNullOrWhiteSpace(appName)) return false;

        string cleanName = CleanRuleName(appName);
        string ruleName = $"{RulePrefix}{cleanName}";

        string args = $"advfirewall firewall delete rule name=\"{ruleName}\"";
        return RunNetsh(args);
    }

    private static string CleanRuleName(string name)
    {
        return name.Replace(".exe", "", StringComparison.OrdinalIgnoreCase)
                   .Replace(" ", "_")
                   .Replace("\"", "")
                   .Trim();
    }

    private static bool RunNetsh(string arguments)
    {
        try
        {
            var psi = new ProcessStartInfo("netsh", arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = true, // Allows UAC elevation if required
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using var proc = Process.Start(psi);
            if (proc != null)
            {
                proc.WaitForExit();
                return proc.ExitCode == 0;
            }
        }
        catch
        {
            // user might have declined UAC prompt
        }

        return false;
    }
}
