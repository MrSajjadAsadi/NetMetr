using System.Windows;
using NetMetr.Helpers;
using NetMetr.Models;
using NetMetr.Services;

namespace NetMetr;

public class HistoryViewModelItem
{
    public string Date { get; set; } = string.Empty;
    public string DownloadFormatted { get; set; } = string.Empty;
    public string UploadFormatted { get; set; } = string.Empty;
    public string TotalFormatted { get; set; } = string.Empty;
}

public partial class SettingsWindow : Window
{
    private readonly AppSettings _originalSettings;
    private readonly UsageTrackerService _usageTracker;
    private readonly NetworkMonitorService _monitorService;

    public AppSettings UpdatedSettings { get; private set; }

    public SettingsWindow(AppSettings settings, UsageTrackerService usageTracker, NetworkMonitorService monitorService)
    {
        InitializeComponent();

        _originalSettings = settings;
        _usageTracker = usageTracker;
        _monitorService = monitorService;

        // Clone current settings
        UpdatedSettings = new AppSettings
        {
            WindowLeft = settings.WindowLeft,
            WindowTop = settings.WindowTop,
            AlwaysOnTop = settings.AlwaysOnTop,
            IsLocked = settings.IsLocked,
            IsCompactMode = settings.IsCompactMode,
            WindowOpacity = settings.WindowOpacity,
            Unit = settings.Unit,
            RefreshIntervalMs = settings.RefreshIntervalMs,
            SelectedAdapterId = settings.SelectedAdapterId,
            PingTargetHost = settings.PingTargetHost,
            EnablePing = settings.EnablePing,
            StartWithWindows = settings.StartWithWindows,
            SnapToEdges = settings.SnapToEdges
        };

        LoadUiValues();
        LoadHistoryData();
    }

    private void LoadUiValues()
    {
        ChkAlwaysOnTop.IsChecked = UpdatedSettings.AlwaysOnTop;
        ChkLockPosition.IsChecked = UpdatedSettings.IsLocked;
        ChkSnapEdges.IsChecked = UpdatedSettings.SnapToEdges;
        ChkStartOnBoot.IsChecked = AutoStartHelper.IsAutoStartEnabled();

        SliderOpacity.Value = UpdatedSettings.WindowOpacity;
        TxtOpacityValue.Text = $"{(int)(UpdatedSettings.WindowOpacity * 100)}%";
        ChkCompactDefault.IsChecked = UpdatedSettings.IsCompactMode;

        // Adapters
        var adapters = NetworkMonitorService.GetAvailableAdapters();
        CmbAdapters.ItemsSource = adapters;
        var selected = adapters.FirstOrDefault(a => a.Id == UpdatedSettings.SelectedAdapterId) ?? adapters.FirstOrDefault();
        CmbAdapters.SelectedItem = selected;

        // Units
        CmbSpeedUnits.SelectedIndex = UpdatedSettings.Unit == SpeedUnit.BitsPerSecond ? 1 : 0;

        // Refresh Interval
        CmbInterval.SelectedIndex = UpdatedSettings.RefreshIntervalMs switch
        {
            <= 500 => 0,
            >= 2000 => 2,
            _ => 1
        };

        // Ping
        ChkEnablePing.IsChecked = UpdatedSettings.EnablePing;
        TxtPingHost.Text = UpdatedSettings.PingTargetHost;
    }

    private void LoadHistoryData()
    {
        var records = _usageTracker.GetHistory(30);
        var today = _usageTracker.GetTodayUsage();

        TxtCardToday.Text = Formatter.FormatDataSize(today.TotalBytes);

        var sevenDaysTotal = records.Take(7).Sum(r => r.TotalBytes);
        TxtCard7Days.Text = Formatter.FormatDataSize(sevenDaysTotal);

        var thirtyDaysTotal = records.Sum(r => r.TotalBytes);
        TxtCard30Days.Text = Formatter.FormatDataSize(thirtyDaysTotal);

        var viewModelList = records.Select(r => new HistoryViewModelItem
        {
            Date = r.DateKey,
            DownloadFormatted = Formatter.FormatDataSize(r.DownloadBytes),
            UploadFormatted = Formatter.FormatDataSize(r.UploadBytes),
            TotalFormatted = Formatter.FormatDataSize(r.TotalBytes)
        }).ToList();

        GridHistory.ItemsSource = viewModelList;
    }

    private void SliderOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (TxtOpacityValue != null)
        {
            TxtOpacityValue.Text = $"{(int)(e.NewValue * 100)}%";
        }
    }

    private void BtnResetToday_Click(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show("Are you sure you want to reset today's statistics?", "Reset Today",
            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
            _usageTracker.ResetTodayUsage();
            LoadHistoryData();
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        UpdatedSettings.AlwaysOnTop = ChkAlwaysOnTop.IsChecked == true;
        UpdatedSettings.IsLocked = ChkLockPosition.IsChecked == true;
        UpdatedSettings.SnapToEdges = ChkSnapEdges.IsChecked == true;
        UpdatedSettings.WindowOpacity = SliderOpacity.Value;
        UpdatedSettings.IsCompactMode = ChkCompactDefault.IsChecked == true;

        if (CmbAdapters.SelectedItem is NetworkAdapterInfo selectedAdapter)
        {
            UpdatedSettings.SelectedAdapterId = selectedAdapter.Id;
        }

        UpdatedSettings.Unit = CmbSpeedUnits.SelectedIndex == 1 ? SpeedUnit.BitsPerSecond : SpeedUnit.BytesPerSecond;

        UpdatedSettings.RefreshIntervalMs = CmbInterval.SelectedIndex switch
        {
            0 => 500,
            2 => 2000,
            _ => 1000
        };

        UpdatedSettings.EnablePing = ChkEnablePing.IsChecked == true;
        UpdatedSettings.PingTargetHost = string.IsNullOrWhiteSpace(TxtPingHost.Text) ? "8.8.8.8" : TxtPingHost.Text.Trim();

        bool startWithWindows = ChkStartOnBoot.IsChecked == true;
        UpdatedSettings.StartWithWindows = startWithWindows;
        AutoStartHelper.SetAutoStart(startWithWindows);

        _usageTracker.SaveSettings(UpdatedSettings);

        DialogResult = true;
        Close();
    }
}
