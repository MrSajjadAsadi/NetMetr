package com.netmetr.app

import android.Manifest
import android.content.pm.PackageManager
import android.os.Build
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Assessment
import androidx.compose.material.icons.filled.Security
import androidx.compose.material.icons.filled.Settings
import androidx.compose.material.icons.filled.Speed
import androidx.compose.material3.Icon
import androidx.compose.material3.NavigationBar
import androidx.compose.material3.NavigationBarItem
import androidx.compose.material3.NavigationBarItemDefaults
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.CompositionLocalProvider
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalLayoutDirection
import androidx.compose.ui.unit.LayoutDirection
import androidx.core.content.ContextCompat
import com.netmetr.app.data.AppFirewallManager
import com.netmetr.app.data.AppNetworkManager
import com.netmetr.app.data.SettingsData
import com.netmetr.app.data.UsageTracker
import com.netmetr.app.service.NetworkSpeedService
import com.netmetr.app.ui.screens.AppsScreen
import com.netmetr.app.ui.screens.DashboardScreen
import com.netmetr.app.ui.screens.HistoryScreen
import com.netmetr.app.ui.screens.SettingsScreen
import com.netmetr.app.ui.theme.AccentSky
import com.netmetr.app.ui.theme.BgDark
import com.netmetr.app.ui.theme.NetMetrTheme
import com.netmetr.app.ui.theme.SurfaceDark
import com.netmetr.app.ui.theme.TextMuted
import com.netmetr.app.util.AppStrings

class MainActivity : ComponentActivity() {

    private lateinit var usageTracker: UsageTracker
    private lateinit var appNetworkManager: AppNetworkManager
    private lateinit var firewallManager: AppFirewallManager

    private val requestPermissionLauncher = registerForActivityResult(
        ActivityResultContracts.RequestPermission()
    ) { isGranted ->
        if (isGranted) {
            NetworkSpeedService.startService(this)
        }
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        usageTracker = UsageTracker(this)
        appNetworkManager = AppNetworkManager(this)
        firewallManager = AppFirewallManager(this)

        checkAndRequestPermissions()

        setContent {
            NetMetrTheme {
                MainAppScreen(
                    usageTracker = usageTracker,
                    appNetworkManager = appNetworkManager,
                    firewallManager = firewallManager
                )
            }
        }
    }

    override fun onResume() {
        super.onResume()
        appNetworkManager.startMonitoring()
    }

    override fun onPause() {
        super.onPause()
        appNetworkManager.stopMonitoring()
    }

    private fun checkAndRequestPermissions() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.TIRAMISU) {
            if (ContextCompat.checkSelfPermission(this, Manifest.permission.POST_NOTIFICATIONS)
                != PackageManager.PERMISSION_GRANTED) {
                requestPermissionLauncher.launch(Manifest.permission.POST_NOTIFICATIONS)
                return
            }
        }

        // Start service by default
        NetworkSpeedService.startService(this)
    }
}

enum class ScreenTab {
    DASHBOARD,
    APPS,
    HISTORY,
    SETTINGS
}

@Composable
fun MainAppScreen(
    usageTracker: UsageTracker,
    appNetworkManager: AppNetworkManager,
    firewallManager: AppFirewallManager
) {
    var currentTab by remember { mutableStateOf(ScreenTab.DASHBOARD) }
    var settings by remember { mutableStateOf(usageTracker.getSettings()) }
    var historyList by remember { mutableStateOf(usageTracker.getHistoryList()) }

    val speedState by NetworkSpeedService.speedFlow.collectAsState()
    val todayUsage by NetworkSpeedService.todayUsageFlow.collectAsState()
    val isServiceRunning by NetworkSpeedService.isServiceRunning.collectAsState()
    val appsList by appNetworkManager.appsFlow.collectAsState()

    val lang = settings.language
    val layoutDirection = if (lang == "fa") LayoutDirection.Rtl else LayoutDirection.Ltr

    CompositionLocalProvider(LocalLayoutDirection provides layoutDirection) {
        Scaffold(
            modifier = Modifier.fillMaxSize(),
            containerColor = BgDark,
            bottomBar = {
                NavigationBar(
                    containerColor = SurfaceDark
                ) {
                    NavigationBarItem(
                        selected = currentTab == ScreenTab.DASHBOARD,
                        onClick = { currentTab = ScreenTab.DASHBOARD },
                        icon = { Icon(Icons.Default.Speed, contentDescription = "Dashboard") },
                        label = { Text(AppStrings.get("tab_speed", lang)) },
                        colors = NavigationBarItemDefaults.colors(
                            selectedIconColor = AccentSky,
                            selectedTextColor = AccentSky,
                            unselectedIconColor = TextMuted,
                            unselectedTextColor = TextMuted,
                            indicatorColor = SurfaceDark
                        )
                    )

                    NavigationBarItem(
                        selected = currentTab == ScreenTab.APPS,
                        onClick = { currentTab = ScreenTab.APPS },
                        icon = { Icon(Icons.Default.Security, contentDescription = "Apps") },
                        label = { Text(AppStrings.get("tab_apps", lang)) },
                        colors = NavigationBarItemDefaults.colors(
                            selectedIconColor = AccentSky,
                            selectedTextColor = AccentSky,
                            unselectedIconColor = TextMuted,
                            unselectedTextColor = TextMuted,
                            indicatorColor = SurfaceDark
                        )
                    )

                    NavigationBarItem(
                        selected = currentTab == ScreenTab.HISTORY,
                        onClick = {
                            historyList = usageTracker.getHistoryList()
                            currentTab = ScreenTab.HISTORY
                        },
                        icon = { Icon(Icons.Default.Assessment, contentDescription = "History") },
                        label = { Text(AppStrings.get("tab_history", lang)) },
                        colors = NavigationBarItemDefaults.colors(
                            selectedIconColor = AccentSky,
                            selectedTextColor = AccentSky,
                            unselectedIconColor = TextMuted,
                            unselectedTextColor = TextMuted,
                            indicatorColor = SurfaceDark
                        )
                    )

                    NavigationBarItem(
                        selected = currentTab == ScreenTab.SETTINGS,
                        onClick = { currentTab = ScreenTab.SETTINGS },
                        icon = { Icon(Icons.Default.Settings, contentDescription = "Settings") },
                        label = { Text(AppStrings.get("tab_settings", lang)) },
                        colors = NavigationBarItemDefaults.colors(
                            selectedIconColor = AccentSky,
                            selectedTextColor = AccentSky,
                            unselectedIconColor = TextMuted,
                            unselectedTextColor = TextMuted,
                            indicatorColor = SurfaceDark
                        )
                    )
                }
            }
        ) { innerPadding ->
            Box(modifier = Modifier.padding(innerPadding)) {
                when (currentTab) {
                    ScreenTab.DASHBOARD -> DashboardScreen(
                        speedState = speedState,
                        todayUsage = todayUsage,
                        isServiceRunning = isServiceRunning,
                        useBits = settings.useBits,
                        lang = lang
                    )
                    ScreenTab.APPS -> AppsScreen(
                        appsList = appsList,
                        firewallManager = firewallManager,
                        onAppBlockedChanged = { pkg, blocked ->
                            firewallManager.setAppBlocked(pkg, blocked)
                        },
                        lang = lang
                    )
                    ScreenTab.HISTORY -> HistoryScreen(
                        historyList = historyList,
                        lang = lang
                    )
                    ScreenTab.SETTINGS -> SettingsScreen(
                        settings = settings,
                        onSettingsChanged = { updated ->
                            settings = updated
                            usageTracker.saveSettings(updated)
                        },
                        onResetToday = {
                            usageTracker.resetToday()
                            historyList = usageTracker.getHistoryList()
                        }
                    )
                }
            }
        }
    }
}
