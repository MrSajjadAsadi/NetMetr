using System.Windows.Forms;
using NetMetr.Helpers;
using NetMetr.Models;

namespace NetMetr.Services;

public class TrayService : IDisposable
{
    private readonly NotifyIcon _notifyIcon;
    private readonly ToolStripMenuItem _statusItem;
    private readonly ToolStripMenuItem _showHideItem;
    private readonly ToolStripMenuItem _compactItem;
    private readonly ToolStripMenuItem _resetItem;
    private readonly ToolStripMenuItem _appsItem;
    private readonly ToolStripMenuItem _settingsItem;
    private readonly ToolStripMenuItem _exitItem;

    public event Action? OnShowHideRequested;
    public event Action? OnToggleCompactRequested;
    public event Action? OnAppsRequested;
    public event Action? OnSettingsRequested;
    public event Action? OnResetTodayRequested;
    public event Action? OnExitRequested;

    public TrayService()
    {
        _notifyIcon = new NotifyIcon
        {
            Icon = IconHelper.CreateAppIcon(),
            Text = LocalizationManager.Get("AppName"),
            Visible = true
        };

        var menu = new ContextMenuStrip();

        _statusItem = new ToolStripMenuItem("NetMetr") { Enabled = false };
        menu.Items.Add(_statusItem);
        menu.Items.Add(new ToolStripSeparator());

        _showHideItem = new ToolStripMenuItem(LocalizationManager.Get("ShowHideWidget"), null, (s, e) => OnShowHideRequested?.Invoke());
        _compactItem = new ToolStripMenuItem(LocalizationManager.Get("CompactMode"), null, (s, e) => OnToggleCompactRequested?.Invoke()) { CheckOnClick = true };
        _resetItem = new ToolStripMenuItem(LocalizationManager.Get("ResetToday"), null, (s, e) => OnResetTodayRequested?.Invoke());
        _appsItem = new ToolStripMenuItem(LocalizationManager.Get("AppManager"), null, (s, e) => OnAppsRequested?.Invoke());
        _settingsItem = new ToolStripMenuItem(LocalizationManager.Get("SettingsAndHistory"), null, (s, e) => OnSettingsRequested?.Invoke());
        _exitItem = new ToolStripMenuItem(LocalizationManager.Get("Exit"), null, (s, e) => OnExitRequested?.Invoke());

        menu.Items.Add(_showHideItem);
        menu.Items.Add(_compactItem);
        menu.Items.Add(_resetItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(_appsItem);
        menu.Items.Add(_settingsItem);
        menu.Items.Add(_exitItem);

        _notifyIcon.ContextMenuStrip = menu;
        _notifyIcon.DoubleClick += (s, e) => OnShowHideRequested?.Invoke();

        LocalizationManager.OnLanguageChanged += RefreshMenuTexts;
        RefreshMenuTexts();
    }

    public void RefreshMenuTexts()
    {
        _showHideItem.Text = LocalizationManager.Get("ShowHideWidget");
        _compactItem.Text = LocalizationManager.Get("CompactMode");
        _resetItem.Text = LocalizationManager.Get("ResetToday");
        _appsItem.Text = LocalizationManager.Get("AppManager");
        _settingsItem.Text = LocalizationManager.Get("SettingsAndHistory");
        _exitItem.Text = LocalizationManager.Get("Exit");
    }

    public void UpdateSpeed(string downSpeed, string upSpeed, bool isCompact)
    {
        _compactItem.Checked = isCompact;
        string text = $"↓ {downSpeed}   ↑ {upSpeed}";
        _statusItem.Text = text;

        // Tooltip max length is 63 chars on Windows
        string tooltip = $"{LocalizationManager.Get("AppName")}\n↓ {downSpeed}\n↑ {upSpeed}";
        if (tooltip.Length > 63)
            tooltip = tooltip.Substring(0, 63);
        _notifyIcon.Text = tooltip;
    }

    public void Dispose()
    {
        LocalizationManager.OnLanguageChanged -= RefreshMenuTexts;
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }
}
