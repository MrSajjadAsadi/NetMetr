using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using NetMetr.Models;
using NetMetr.Services;
using NetMetr.Helpers;

namespace NetMetr;

public partial class AppsWindow : Window
{
    private readonly AppNetworkMonitorService _monitorService;
    private List<AppNetworkInfo> _latestApps = new();
    private string _filterQuery = string.Empty;

    public AppsWindow()
    {
        InitializeComponent();
        _monitorService = new AppNetworkMonitorService(1500);
        _monitorService.AppsUpdated += MonitorService_AppsUpdated;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        ApplyLocalization();
        LocalizationManager.OnLanguageChanged += ApplyLocalization;
        _monitorService.Start();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        LocalizationManager.OnLanguageChanged -= ApplyLocalization;
        _monitorService.Stop();
        _monitorService.Dispose();
    }

    private void ApplyLocalization()
    {
        FlowDirection = LocalizationManager.CurrentFlowDirection;
        Title = LocalizationManager.Get("AppsTitle");

        TxtHeaderTitle.Text = LocalizationManager.Get("AppsHeader");
        TxtHeaderSub.Text = LocalizationManager.Get("AppsSubheader");
        TxtLblActiveApps.Text = LocalizationManager.Get("ActiveAppsCount");

        RadioFilterAll.Content = LocalizationManager.Get("FilterAll");
        RadioFilterActive.Content = LocalizationManager.Get("FilterActive");
        RadioFilterBlocked.Content = LocalizationManager.Get("FilterBlocked");

        ColHeaderApp.Header = LocalizationManager.Get("ColApplication");
        ColHeaderDown.Header = LocalizationManager.Get("ColDownload");
        ColHeaderUp.Header = LocalizationManager.Get("ColUpload");
        ColHeaderTotal.Header = LocalizationManager.Get("ColTotal");
        ColHeaderSockets.Header = LocalizationManager.Get("ColSockets");
        ColHeaderStatus.Header = LocalizationManager.Get("ColStatus");
        ColHeaderControl.Header = LocalizationManager.Get("ColControl");

        TxtFirewallTip.Text = LocalizationManager.Get("FirewallTip");
        BtnClose.Content = LocalizationManager.Get("Close");
    }

    private void MonitorService_AppsUpdated(List<AppNetworkInfo> apps)
    {
        Dispatcher.Invoke(() =>
        {
            _latestApps = apps;
            ApplyFilter();
            TxtTotalApps.Text = apps.Count.ToString();
        });
    }

    private void ApplyFilter()
    {
        var filtered = _latestApps.AsEnumerable();

        if (RadioFilterActive.IsChecked == true)
        {
            filtered = filtered.Where(a => a.DownloadSpeedBytesPerSec > 0 || a.UploadSpeedBytesPerSec > 0);
        }
        else if (RadioFilterBlocked.IsChecked == true)
        {
            filtered = filtered.Where(a => a.IsBlocked);
        }

        if (!string.IsNullOrWhiteSpace(_filterQuery))
        {
            string q = _filterQuery.Trim().ToLowerInvariant();
            filtered = filtered.Where(a =>
                a.ProcessName.ToLowerInvariant().Contains(q) ||
                a.ProcessId.ToString().Contains(q));
        }

        GridApps.ItemsSource = filtered.ToList();
    }

    private void Filter_Checked(object sender, RoutedEventArgs e)
    {
        ApplyFilter();
    }

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        _filterQuery = TxtSearch.Text;
        ApplyFilter();
    }

    private void BtnToggleBlock_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is AppNetworkInfo app)
        {
            bool isFa = LocalizationManager.CurrentLanguage == "fa";
            if (app.IsBlocked)
            {
                bool success = FirewallManager.UnblockApp(app.ProcessName);
                if (success)
                {
                    app.IsBlocked = false;
                    string msg = isFa ? $"دسترسی اینترنت '{app.ProcessName}' مجدداً برقرار شد." : $"Unblocked {app.ProcessName} from accessing the internet.";
                    MessageBox.Show(msg, LocalizationManager.Get("AppName"), MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    string msg = isFa ? $"خطا در وصل کردن اینترنت '{app.ProcessName}'. لطفاً برنامه را با دسترسی Administrator اجرا کنید." : $"Failed to unblock {app.ProcessName}. Make sure to grant UAC permission if requested.";
                    MessageBox.Show(msg, LocalizationManager.Get("AppName"), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(app.ExecutablePath))
                {
                    string msg = isFa ? $"امکان مسدودسازی '{app.ProcessName}' وجود ندارد زیرا این برنامه سیستمی حفاظت‌شده است." : $"Cannot block {app.ProcessName} because its executable path could not be resolved (protected system process).";
                    MessageBox.Show(msg, LocalizationManager.Get("AppName"), MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string prompt = isFa ? $"آیا از قطع دسترسی اینترنت نرم‌افزار '{app.ProcessName}' اطمینان دارید؟\n\nبا این کار هرگونه ارسال و دریافت دیتا توسط این برنامه مسدود خواهد شد." : $"Block internet access for '{app.ProcessName}'?\n\nThis will prevent all inbound and outbound traffic for this application.";
                string title = isFa ? "تأیید مسدودسازی فایروال" : "Confirm Block";
                if (MessageBox.Show(prompt, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    bool success = FirewallManager.BlockApp(app.ExecutablePath, app.ProcessName);
                    if (success)
                    {
                        app.IsBlocked = true;
                        string msg = isFa ? $"دسترسی اینترنت '{app.ProcessName}' در فایروال ویندوز مسدود شد." : $"Blocked {app.ProcessName} in Windows Firewall.";
                        MessageBox.Show(msg, LocalizationManager.Get("AppName"), MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        string msg = isFa ? $"خطا در مسدودسازی '{app.ProcessName}'. لطفاً برنامه را با دسترسی Run as Administrator اجرا کنید." : $"Failed to block {app.ProcessName}. Please run NetMetr as Administrator.";
                        MessageBox.Show(msg, LocalizationManager.Get("AppName"), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }

    private void BtnKillProcess_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is AppNetworkInfo app)
        {
            bool isFa = LocalizationManager.CurrentLanguage == "fa";
            string prompt = isFa ? $"آیا از بستن اجباری پروسس '{app.ProcessName}' (PID: {app.ProcessId}) مطمئن هستید؟" : $"Are you sure you want to terminate '{app.ProcessName}' (PID: {app.ProcessId})?";
            string title = isFa ? "بستن برنامه" : "End Task";
            if (MessageBox.Show(prompt, title, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    var proc = Process.GetProcessById(app.ProcessId);
                    proc.Kill();
                    _latestApps.Remove(app);
                    ApplyFilter();
                }
                catch (Exception ex)
                {
                    string err = isFa ? $"خطا در بستن برنامه: {ex.Message}" : $"Could not terminate process: {ex.Message}";
                    MessageBox.Show(err, LocalizationManager.Get("AppName"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
