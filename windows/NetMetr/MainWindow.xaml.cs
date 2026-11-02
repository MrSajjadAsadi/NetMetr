using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using NetMetr.Helpers;
using NetMetr.Models;
using NetMetr.Services;

namespace NetMetr;

public partial class MainWindow : Window
{
    private readonly UsageTrackerService _usageTracker;
    private readonly NetworkMonitorService _monitorService;
    private readonly TrayService _trayService;
    private AppSettings _settings;

    private readonly List<double> _downHistory = new();
    private readonly List<double> _upHistory = new();
    private const int MaxHistoryPoints = 35;
    private readonly DispatcherTimer _usageSaveTimer;

    public MainWindow()
    {
        InitializeComponent();

        _usageTracker = new UsageTrackerService();
        _settings = _usageTracker.LoadSettings();

        _monitorService = new NetworkMonitorService(_usageTracker, _settings.RefreshIntervalMs)
        {
            SelectedAdapterId = _settings.SelectedAdapterId,
            PingTargetHost = _settings.PingTargetHost,
            EnablePing = _settings.EnablePing
        };

        _trayService = new TrayService();
        _trayService.OnShowHideRequested += Tray_OnShowHideRequested;
        _trayService.OnToggleCompactRequested += Tray_OnToggleCompactRequested;
        _trayService.OnAppsRequested += OpenAppsWindow;
        _trayService.OnSettingsRequested += OpenSettings;
        _trayService.OnResetTodayRequested += ResetTodayStats;
        _trayService.OnExitRequested += () => Application.Current.Shutdown();

        _monitorService.SpeedUpdated += MonitorService_SpeedUpdated;

        // Auto-save usage every 30 seconds
        _usageSaveTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        _usageSaveTimer.Tick += (s, e) => _usageTracker.SaveUsage();
        _usageSaveTimer.Start();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        LocalizationManager.SetLanguage(_settings.Language);
        LocalizationManager.OnLanguageChanged += ApplyLocalization;
        ApplySettings();
        ApplyLocalization();
        _monitorService.Start();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        _usageSaveTimer.Stop();
        _monitorService.Stop();
        _monitorService.Dispose();
        _usageTracker.SaveUsage();
        _trayService.Dispose();
    }

    private void ApplySettings()
    {
        Topmost = _settings.AlwaysOnTop;
        MenuAlwaysOnTop.IsChecked = _settings.AlwaysOnTop;
        MenuLockPosition.IsChecked = _settings.IsLocked;
        Opacity = _settings.WindowOpacity;

        SetCompactMode(_settings.IsCompactMode);

        // Position restoring
        var workArea = SystemParameters.WorkArea;
        if (_settings.WindowLeft >= 0 && _settings.WindowTop >= 0 &&
            _settings.WindowLeft < workArea.Right - 50 && _settings.WindowTop < workArea.Bottom - 30)
        {
            Left = _settings.WindowLeft;
            Top = _settings.WindowTop;
        }
        else
        {
            // Default: Top-right corner of work area
            Left = workArea.Right - 340;
            Top = workArea.Top + 40;
        }

        ApplyLocalization();
    }

    private void ApplyLocalization()
    {
        FlowDirection = LocalizationManager.CurrentFlowDirection;
        TxtAppTitle.Text = LocalizationManager.Get("AppName");
        TxtLblDown.Text = "↓ " + LocalizationManager.Get("Download");
        TxtLblUp.Text = "↑ " + LocalizationManager.Get("Upload");
        TxtLblToday.Text = LocalizationManager.Get("Today");
        RunTodayTotalPrefix.Text = LocalizationManager.Get("TotalPrefix");

        BtnExpand.ToolTip = LocalizationManager.Get("TipExpand");
        BtnCompact.ToolTip = LocalizationManager.Get("TipCompact");
        BtnSettings.ToolTip = LocalizationManager.Get("TipSettings");
        BtnApps.ToolTip = LocalizationManager.Get("TipFirewall");

        MenuHeaderTitle.Header = LocalizationManager.Get("AppName") + " - " + LocalizationManager.Get("AppSubtitle");
        MenuCompactToggle.Header = _settings.IsCompactMode ? LocalizationManager.Get("ExpandedMode") : LocalizationManager.Get("CompactMode");
        MenuAlwaysOnTop.Header = LocalizationManager.Get("AlwaysOnTop");
        MenuLockPosition.Header = LocalizationManager.Get("LockPosition");
        MenuOpacityHeader.Header = LocalizationManager.Get("Opacity");
        MenuUnitHeader.Header = LocalizationManager.Get("SpeedUnit");
        if (MenuUnitHeader.Items.Count >= 2)
        {
            if (MenuUnitHeader.Items[0] is MenuItem bytesItem) bytesItem.Header = LocalizationManager.Get("BytesUnit");
            if (MenuUnitHeader.Items[1] is MenuItem bitsItem) bitsItem.Header = LocalizationManager.Get("BitsUnit");
        }
        MenuAppsItem.Header = LocalizationManager.Get("AppManager");
        MenuSettingsItem.Header = LocalizationManager.Get("SettingsAndHistory");
        MenuResetItem.Header = LocalizationManager.Get("ResetToday");
        MenuMinimizeItem.Header = LocalizationManager.Get("MinimizeToTray");
        MenuExitItem.Header = LocalizationManager.Get("Exit");
    }

    private void MonitorService_SpeedUpdated(NetworkSpeed speed)
    {
        Dispatcher.Invoke(() =>
        {
            string downStr = Formatter.FormatSpeed(speed.DownloadBytesPerSec, _settings.Unit);
            string upStr = Formatter.FormatSpeed(speed.UploadBytesPerSec, _settings.Unit);

            TxtDownSpeed.Text = downStr;
            TxtUpSpeed.Text = upStr;
            TxtCompactDown.Text = downStr;
            TxtCompactUp.Text = upStr;

            TxtAdapterName.Text = speed.ActiveInterfaceName;

            // Ping display & indicator color
            string pingStr = Formatter.FormatLatency(speed.LatencyMs, speed.IsConnected);
            TxtPing.Text = pingStr;
            TxtCompactPing.Text = pingStr;

            Brush pingBrush;
            if (!speed.IsConnected)
            {
                pingBrush = new SolidColorBrush(Color.FromRgb(239, 68, 68)); // Red (disconnected)
            }
            else if (speed.LatencyMs < 0)
            {
                pingBrush = new SolidColorBrush(Color.FromRgb(56, 189, 248)); // Sky blue (connected, waiting/blocked ping)
            }
            else if (speed.LatencyMs < 70)
            {
                pingBrush = new SolidColorBrush(Color.FromRgb(16, 185, 129)); // Green (good)
            }
            else if (speed.LatencyMs < 160)
            {
                pingBrush = new SolidColorBrush(Color.FromRgb(245, 158, 11)); // Amber (average)
            }
            else
            {
                pingBrush = new SolidColorBrush(Color.FromRgb(239, 68, 68)); // Red (high latency)
            }
            DotPing.Fill = pingBrush;
            DotCompactPing.Fill = pingBrush;

            // Today usage display
            var today = _usageTracker.GetTodayUsage();
            TxtTodayDown.Text = Formatter.FormatDataSize(today.DownloadBytes);
            TxtTodayUp.Text = Formatter.FormatDataSize(today.UploadBytes);
            TxtTodayTotal.Text = Formatter.FormatDataSize(today.TotalBytes);

            // Update sparkline graph
            UpdateGraph(speed.DownloadBytesPerSec, speed.UploadBytesPerSec);

            // Update system tray tooltip
            _trayService.UpdateSpeed(downStr, upStr, _settings.IsCompactMode);
        });
    }

    private void UpdateGraph(double downBytes, double upBytes)
    {
        if (ExpandedView.Visibility != Visibility.Visible) return;

        _downHistory.Add(downBytes);
        _upHistory.Add(upBytes);

        if (_downHistory.Count > MaxHistoryPoints) _downHistory.RemoveAt(0);
        if (_upHistory.Count > MaxHistoryPoints) _upHistory.RemoveAt(0);

        double width = GraphCanvas.ActualWidth > 0 ? GraphCanvas.ActualWidth : 280;
        double height = GraphCanvas.ActualHeight > 0 ? GraphCanvas.ActualHeight : 42;

        double maxVal = Math.Max(1024 * 50, Math.Max(_downHistory.Max(), _upHistory.Max()));

        var downPoints = new PointCollection();
        var upPoints = new PointCollection();

        for (int i = 0; i < _downHistory.Count; i++)
        {
            double x = (width / (MaxHistoryPoints - 1)) * i;
            double yDown = height - (_downHistory[i] / maxVal * (height - 4)) - 2;
            double yUp = height - (_upHistory[i] / maxVal * (height - 4)) - 2;

            yDown = Math.Clamp(yDown, 2, height - 2);
            yUp = Math.Clamp(yUp, 2, height - 2);

            downPoints.Add(new Point(x, yDown));
            upPoints.Add(new Point(x, yUp));
        }

        PolylineDownload.Points = downPoints;
        PolylineUpload.Points = upPoints;
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_settings.IsLocked) return;

        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
            SnapAndSavePosition();
        }
    }

    private void SnapAndSavePosition()
    {
        if (!_settings.SnapToEdges)
        {
            _settings.WindowLeft = Left;
            _settings.WindowTop = Top;
            _usageTracker.SaveSettings(_settings);
            return;
        }

        var workArea = SystemParameters.WorkArea;
        const double snapMargin = 20;

        // Snap Left
        if (Math.Abs(Left - workArea.Left) < snapMargin)
            Left = workArea.Left + 4;
        // Snap Right
        else if (Math.Abs((Left + ActualWidth) - workArea.Right) < snapMargin)
            Left = workArea.Right - ActualWidth - 4;

        // Snap Top
        if (Math.Abs(Top - workArea.Top) < snapMargin)
            Top = workArea.Top + 4;
        // Snap Bottom
        else if (Math.Abs((Top + ActualHeight) - workArea.Bottom) < snapMargin)
            Top = workArea.Bottom - ActualHeight - 4;

        _settings.WindowLeft = Left;
        _settings.WindowTop = Top;
        _usageTracker.SaveSettings(_settings);
    }

    private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        ContextMenu.IsOpen = true;
    }

    private void SetCompactMode(bool compact)
    {
        _settings.IsCompactMode = compact;
        MenuCompactToggle.Header = compact ? "Expanded View" : "Compact Mode";

        if (compact)
        {
            ExpandedView.Visibility = Visibility.Collapsed;
            CompactView.Visibility = Visibility.Visible;
            MainBorder.BorderThickness = new Thickness(1);
        }
        else
        {
            CompactView.Visibility = Visibility.Collapsed;
            ExpandedView.Visibility = Visibility.Visible;
            MainBorder.BorderThickness = new Thickness(1.2);
        }

        _usageTracker.SaveSettings(_settings);
    }

    private void BtnToggleCompact_Click(object sender, RoutedEventArgs e)
    {
        SetCompactMode(!_settings.IsCompactMode);
    }

    private void MenuCompactToggle_Click(object sender, RoutedEventArgs e)
    {
        SetCompactMode(!_settings.IsCompactMode);
    }

    private void MenuAlwaysOnTop_Click(object sender, RoutedEventArgs e)
    {
        _settings.AlwaysOnTop = MenuAlwaysOnTop.IsChecked;
        Topmost = _settings.AlwaysOnTop;
        _usageTracker.SaveSettings(_settings);
    }

    private void MenuLockPosition_Click(object sender, RoutedEventArgs e)
    {
        _settings.IsLocked = MenuLockPosition.IsChecked;
        MainBorder.Cursor = _settings.IsLocked ? Cursors.Arrow : Cursors.SizeAll;
        _usageTracker.SaveSettings(_settings);
    }

    private void MenuOpacity_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem item && double.TryParse(item.Tag?.ToString(), out double op))
        {
            _settings.WindowOpacity = op;
            Opacity = op;
            _usageTracker.SaveSettings(_settings);
        }
    }

    private void MenuUnit_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem item)
        {
            _settings.Unit = item.Tag?.ToString() == "Bits" ? SpeedUnit.BitsPerSecond : SpeedUnit.BytesPerSecond;
            _usageTracker.SaveSettings(_settings);
        }
    }

    private AppsWindow? _appsWindow;

    private void MenuApps_Click(object sender, RoutedEventArgs e)
    {
        OpenAppsWindow();
    }

    private void OpenAppsWindow()
    {
        if (_appsWindow == null || !_appsWindow.IsLoaded)
        {
            _appsWindow = new AppsWindow();
            _appsWindow.Closed += (s, e) => _appsWindow = null;
            _appsWindow.Show();
        }
        else
        {
            _appsWindow.Activate();
        }
    }

    private void MenuSettings_Click(object sender, RoutedEventArgs e)
    {
        OpenSettings();
    }

    private void OpenSettings()
    {
        var settingsWin = new SettingsWindow(_settings, _usageTracker, _monitorService);
        settingsWin.Owner = this;
        if (settingsWin.ShowDialog() == true)
        {
            _settings = settingsWin.UpdatedSettings;
            ApplySettings();
            _monitorService.SetInterval(_settings.RefreshIntervalMs);
            _monitorService.SelectedAdapterId = _settings.SelectedAdapterId;
            _monitorService.PingTargetHost = _settings.PingTargetHost;
            _monitorService.EnablePing = _settings.EnablePing;
        }
    }

    private void ResetTodayStats()
    {
        if (MessageBox.Show("Are you sure you want to reset today's usage statistics?", "NetMetr",
            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            _usageTracker.ResetTodayUsage();
            TxtTodayDown.Text = "0 B";
            TxtTodayUp.Text = "0 B";
            TxtTodayTotal.Text = "0 B";
        }
    }

    private void MenuResetToday_Click(object sender, RoutedEventArgs e)
    {
        ResetTodayStats();
    }

    private void MenuMinimizeToTray_Click(object sender, RoutedEventArgs e)
    {
        Hide();
    }

    private void MenuExit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void Tray_OnShowHideRequested()
    {
        if (IsVisible)
        {
            Hide();
        }
        else
        {
            Show();
            Activate();
        }
    }

    private void Tray_OnToggleCompactRequested()
    {
        SetCompactMode(!_settings.IsCompactMode);
    }
}
