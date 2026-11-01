using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using NetMetr.Models;
using NetMetr.Services;

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
        _monitorService.Start();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        _monitorService.Stop();
        _monitorService.Dispose();
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
        if (string.IsNullOrWhiteSpace(_filterQuery))
        {
            GridApps.ItemsSource = _latestApps;
        }
        else
        {
            string q = _filterQuery.Trim().ToLowerInvariant();
            GridApps.ItemsSource = _latestApps.Where(a =>
                a.ProcessName.ToLowerInvariant().Contains(q) ||
                a.ProcessId.ToString().Contains(q)).ToList();
        }
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
            if (app.IsBlocked)
            {
                bool success = FirewallManager.UnblockApp(app.ProcessName);
                if (success)
                {
                    app.IsBlocked = false;
                    MessageBox.Show($"Unblocked {app.ProcessName} from accessing the internet.", "NetMetr Firewall", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Failed to unblock {app.ProcessName}. Make sure to grant UAC permission if requested.", "NetMetr", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(app.ExecutablePath))
                {
                    MessageBox.Show($"Cannot block {app.ProcessName} because its executable path could not be resolved (protected system process).", "NetMetr", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Block internet access for '{app.ProcessName}'?\n\nThis will prevent all inbound and outbound traffic for this application.", "Confirm Block", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    bool success = FirewallManager.BlockApp(app.ExecutablePath, app.ProcessName);
                    if (success)
                    {
                        app.IsBlocked = true;
                        MessageBox.Show($"Blocked {app.ProcessName} in Windows Firewall.", "NetMetr Firewall", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Failed to block {app.ProcessName}. Make sure to accept the UAC administrator prompt.", "NetMetr", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }
    }

    private void BtnKillProcess_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is AppNetworkInfo app)
        {
            if (MessageBox.Show($"Terminate process '{app.ProcessName}' (PID: {app.ProcessId})?", "End Task", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    using var proc = Process.GetProcessById(app.ProcessId);
                    proc.Kill();
                    MessageBox.Show($"Terminated {app.ProcessName}.", "NetMetr", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not terminate process: {ex.Message}", "NetMetr", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
