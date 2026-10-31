using System.Windows.Forms;
using NetMetr.Helpers;
using NetMetr.Models;

namespace NetMetr.Services;

public class TrayService : IDisposable
{
    private readonly NotifyIcon _notifyIcon;
    private readonly ToolStripMenuItem _statusItem;
    private readonly ToolStripMenuItem _compactItem;

    public event Action? OnShowHideRequested;
    public event Action? OnToggleCompactRequested;
    public event Action? OnSettingsRequested;
    public event Action? OnResetTodayRequested;
    public event Action? OnExitRequested;

    public TrayService()
    {
        _notifyIcon = new NotifyIcon
        {
            Icon = IconHelper.CreateAppIcon(),
            Text = "NetMetr - Network Speed Monitor",
            Visible = true
        };

        var menu = new ContextMenuStrip();

        _statusItem = new ToolStripMenuItem("NetMetr: Initializing...") { Enabled = false };
        menu.Items.Add(_statusItem);
        menu.Items.Add(new ToolStripSeparator());

        var showHideItem = new ToolStripMenuItem("Show / Hide Widget", null, (s, e) => OnShowHideRequested?.Invoke());
        _compactItem = new ToolStripMenuItem("Compact Mode", null, (s, e) => OnToggleCompactRequested?.Invoke()) { CheckOnClick = true };
        var resetItem = new ToolStripMenuItem("Reset Today's Stats", null, (s, e) => OnResetTodayRequested?.Invoke());
        var settingsItem = new ToolStripMenuItem("Settings...", null, (s, e) => OnSettingsRequested?.Invoke());
        var exitItem = new ToolStripMenuItem("Exit NetMetr", null, (s, e) => OnExitRequested?.Invoke());

        menu.Items.Add(showHideItem);
        menu.Items.Add(_compactItem);
        menu.Items.Add(resetItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(settingsItem);
        menu.Items.Add(exitItem);

        _notifyIcon.ContextMenuStrip = menu;
        _notifyIcon.DoubleClick += (s, e) => OnShowHideRequested?.Invoke();
    }

    public void UpdateSpeed(string downSpeed, string upSpeed, bool isCompact)
    {
        _compactItem.Checked = isCompact;
        string text = $"↓ {downSpeed}   ↑ {upSpeed}";
        _statusItem.Text = text;

        // Tooltip max length is 63 chars on older Windows
        string tooltip = $"NetMetr\n↓ {downSpeed}\n↑ {upSpeed}";
        if (tooltip.Length > 63)
            tooltip = tooltip.Substring(0, 63);
        _notifyIcon.Text = tooltip;
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }
}
