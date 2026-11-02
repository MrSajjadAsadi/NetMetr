using System.Windows;
using System.Windows.Controls;
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
            SnapToEdges = settings.SnapToEdges,
            Language = settings.Language
        };

        LoadUiValues();
        LoadHistoryData();
        ApplyLocalization();
    }

    private void LoadUiValues()
    {
        CmbLanguage.SelectedIndex = UpdatedSettings.Language == "en" ? 1 : 0;
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

    private void ApplyLocalization()
    {
        FlowDirection = LocalizationManager.CurrentFlowDirection;
        Title = LocalizationManager.Get("SettingsTitle");

        TxtHeaderTitle.Text = LocalizationManager.Get("SettingsHeader");
        TxtHeaderSub.Text = LocalizationManager.Get("SettingsSubheader");

        // Tabs
        TabGeneral.Header = LocalizationManager.Get("GeneralTab");
        TabNetwork.Header = LocalizationManager.Get("NetworkTab");
        TabUsage.Header = LocalizationManager.Get("UsageTab");
        TabAbout.Header = LocalizationManager.Get("AboutTab");

        // General
        TxtLblLangTitle.Text = LocalizationManager.Get("Language");
        TxtBehaviorTitle.Text = LocalizationManager.Get("BehaviorSection");
        ChkAlwaysOnTop.Content = LocalizationManager.Get("AlwaysOnTopDesc");
        ChkLockPosition.Content = LocalizationManager.Get("LockPosDesc");
        ChkSnapEdges.Content = LocalizationManager.Get("SnapEdgesDesc");
        ChkStartOnBoot.Content = LocalizationManager.Get("StartOnBootDesc");
        TxtTransparencyTitle.Text = LocalizationManager.Get("TransparencySection");
        TxtLblOpacity.Text = LocalizationManager.Get("OpacityLabel");
        ChkCompactDefault.Content = LocalizationManager.Get("CompactDefaultDesc");

        // Network
        TxtInterfaceTitle.Text = LocalizationManager.Get("NetworkSettingsSection");
        TxtLblAdapter.Text = LocalizationManager.Get("AdapterLabel");
        TxtLblSpeedUnit.Text = LocalizationManager.Get("SpeedUnitLabel");
        CmbItemBytes.Content = LocalizationManager.Get("UnitBytesDesc");
        CmbItemBits.Content = LocalizationManager.Get("UnitBitsDesc");
        TxtLblInterval.Text = LocalizationManager.Get("RefreshRateLabel");
        CmbItemFast.Content = LocalizationManager.Get("FastInterval");
        CmbItemNormal.Content = LocalizationManager.Get("NormalInterval");
        CmbItemRelaxed.Content = LocalizationManager.Get("RelaxedInterval");
        TxtPingTitle.Text = LocalizationManager.Get("PingSection");
        ChkEnablePing.Content = LocalizationManager.Get("EnablePingDesc");
        TxtLblPingHost.Text = LocalizationManager.Get("PingHostLabel");

        // Usage History
        TxtLblCardToday.Text = LocalizationManager.Get("Today");
        TxtLblCard7Days.Text = LocalizationManager.CurrentLanguage == "fa" ? "۷ روز گذشته" : "Last 7 Days";
        TxtLblCard30Days.Text = LocalizationManager.CurrentLanguage == "fa" ? "۳۰ روز گذشته" : "Last 30 Days";
        ColGridDate.Header = LocalizationManager.Get("ColDate");
        ColGridDown.Header = LocalizationManager.Get("Download");
        ColGridUp.Header = LocalizationManager.Get("Upload");
        ColGridTotal.Header = LocalizationManager.Get("ColTotalUsage");

        // About
        TxtAboutDesc.Text = LocalizationManager.Get("AboutDesc");
        TxtAboutFramework.Text = LocalizationManager.Get("AboutFramework");

        // Buttons
        BtnResetTodayStats.Content = LocalizationManager.Get("ResetToday");
        BtnCancel.Content = LocalizationManager.Get("Cancel");
        BtnSave.Content = LocalizationManager.Get("SaveApply");
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

    private void CmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbLanguage == null || !IsLoaded) return;
        UpdatedSettings.Language = CmbLanguage.SelectedIndex == 1 ? "en" : "fa";
        LocalizationManager.SetLanguage(UpdatedSettings.Language);
        ApplyLocalization();
    }

    private void BtnResetToday_Click(object sender, RoutedEventArgs e)
    {
        string prompt = LocalizationManager.Get("ConfirmReset");
        string title = LocalizationManager.Get("ResetToday");
        if (MessageBox.Show(prompt, title, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
            _usageTracker.ResetTodayUsage();
            LoadHistoryData();
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        // Revert language if cancelled
        LocalizationManager.SetLanguage(_originalSettings.Language);
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
        UpdatedSettings.PingTargetHost = string.IsNullOrWhiteSpace(TxtPingHost.Text) ? "1.1.1.1" : TxtPingHost.Text.Trim();

        // Auto start
        bool wantAutoStart = ChkStartOnBoot.IsChecked == true;
        AutoStartHelper.SetAutoStart(wantAutoStart);
        UpdatedSettings.StartWithWindows = wantAutoStart;

        LocalizationManager.SetLanguage(UpdatedSettings.Language);

        DialogResult = true;
        Close();
    }
}
