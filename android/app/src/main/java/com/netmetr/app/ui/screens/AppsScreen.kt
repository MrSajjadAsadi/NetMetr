package com.netmetr.app.ui.screens

import android.app.Activity
import android.net.VpnService
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Search
import androidx.compose.material.icons.filled.Settings
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.FilterChip
import androidx.compose.material3.FilterChipDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.OutlinedTextFieldDefaults
import androidx.compose.material3.Switch
import androidx.compose.material3.SwitchDefaults
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.core.graphics.drawable.toBitmap
import com.netmetr.app.data.AppFirewallManager
import com.netmetr.app.data.AppUsageInfo
import com.netmetr.app.ui.theme.AccentGreen
import com.netmetr.app.ui.theme.AccentRed
import com.netmetr.app.ui.theme.AccentSky
import com.netmetr.app.ui.theme.PrimaryBlue
import com.netmetr.app.ui.theme.SurfaceCard
import com.netmetr.app.ui.theme.SurfaceCardBorder
import com.netmetr.app.ui.theme.TextMuted
import com.netmetr.app.ui.theme.TextPrimary
import com.netmetr.app.ui.theme.TextSecondary
import com.netmetr.app.util.AppStrings
import com.netmetr.app.util.SpeedFormatter

enum class AppFilter {
    ALL,
    ACTIVE_ONLY,
    BLOCKED_ONLY
}

@Composable
fun AppsScreen(
    appsList: List<AppUsageInfo>,
    firewallManager: AppFirewallManager,
    onAppBlockedChanged: (String, Boolean) -> Unit,
    lang: String = "fa"
) {
    val context = LocalContext.current
    var searchQuery by remember { mutableStateOf("") }
    var selectedFilter by remember { mutableStateOf<AppFilter>(AppFilter.ALL) }
    var isFirewallRunning by remember { mutableStateOf(firewallManager.isFirewallEnabled()) }

    // Launcher for VPN permission
    val vpnLauncher = rememberLauncherForActivityResult(
        contract = ActivityResultContracts.StartActivityForResult()
    ) { result ->
        if (result.resultCode == Activity.RESULT_OK) {
            isFirewallRunning = true
            firewallManager.setFirewallEnabled(true)
        }
    }

    val filteredApps = remember(appsList, searchQuery, selectedFilter) {
        appsList.filter { app ->
            val matchesQuery = searchQuery.isBlank() ||
                    app.appName.contains(searchQuery, ignoreCase = true) ||
                    app.packageName.contains(searchQuery, ignoreCase = true)

            val matchesFilter = when (selectedFilter) {
                AppFilter.ALL -> true
                AppFilter.ACTIVE_ONLY -> app.totalSpeedBytes > 0
                AppFilter.BLOCKED_ONLY -> app.isBlocked
            }

            matchesQuery && matchesFilter
        }
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(16.dp)
    ) {
        // Header
        Text(
            text = AppStrings.get("apps_title", lang),
            color = TextPrimary,
            fontWeight = FontWeight.ExtraBold,
            fontSize = 24.sp
        )
        Text(
            text = AppStrings.get("apps_subtitle", lang),
            color = TextSecondary,
            fontSize = 13.sp
        )

        Spacer(modifier = Modifier.height(14.dp))

        // Firewall Master Switch Card
        Card(
            modifier = Modifier.fillMaxWidth(),
            shape = RoundedCornerShape(14.dp),
            colors = CardDefaults.cardColors(
                containerColor = if (isFirewallRunning) PrimaryBlue.copy(alpha = 0.15f) else SurfaceCard
            ),
            border = BorderStroke(
                1.dp,
                if (isFirewallRunning) PrimaryBlue.copy(alpha = 0.5f) else SurfaceCardBorder
            )
        ) {
            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(14.dp),
                verticalAlignment = Alignment.CenterVertically,
                horizontalArrangement = Arrangement.SpaceBetween
            ) {
                Column(modifier = Modifier.weight(1f)) {
                    Text(
                        text = if (isFirewallRunning) AppStrings.get("firewall_active", lang) else AppStrings.get("firewall_disabled", lang),
                        color = TextPrimary,
                        fontWeight = FontWeight.Bold,
                        fontSize = 15.sp
                    )
                    Text(
                        text = if (isFirewallRunning)
                            AppStrings.get("firewall_active_desc", lang)
                        else
                            AppStrings.get("firewall_disabled_desc", lang),
                        color = TextSecondary,
                        fontSize = 12.sp
                    )
                }

                Switch(
                    checked = isFirewallRunning,
                    onCheckedChange = { enable ->
                        if (enable) {
                            val vpnIntent = VpnService.prepare(context)
                            if (vpnIntent != null) {
                                vpnLauncher.launch(vpnIntent)
                            } else {
                                isFirewallRunning = true
                                firewallManager.setFirewallEnabled(true)
                            }
                        } else {
                            isFirewallRunning = false
                            firewallManager.setFirewallEnabled(false)
                        }
                    },
                    colors = SwitchDefaults.colors(
                        checkedTrackColor = PrimaryBlue,
                        uncheckedTrackColor = SurfaceCardBorder
                    )
                )
            }
        }

        Spacer(modifier = Modifier.height(12.dp))

        // Search Bar
        OutlinedTextField(
            value = searchQuery,
            onValueChange = { searchQuery = it },
            modifier = Modifier.fillMaxWidth(),
            placeholder = { Text(AppStrings.get("search_apps", lang), color = TextMuted) },
            leadingIcon = { Icon(Icons.Default.Search, contentDescription = "Search", tint = TextMuted) },
            singleLine = true,
            shape = RoundedCornerShape(12.dp),
            colors = OutlinedTextFieldDefaults.colors(
                focusedBorderColor = AccentSky,
                unfocusedBorderColor = SurfaceCardBorder,
                focusedContainerColor = SurfaceCard,
                unfocusedContainerColor = SurfaceCard,
                focusedTextColor = TextPrimary,
                unfocusedTextColor = TextPrimary
            )
        )

        Spacer(modifier = Modifier.height(10.dp))

        // Filter Chips
        Row(
            modifier = Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.spacedBy(8.dp)
        ) {
            FilterChip(
                selected = selectedFilter == AppFilter.ALL,
                onClick = { selectedFilter = AppFilter.ALL },
                label = { Text("${AppStrings.get("filter_all", lang)} (${appsList.size})") },
                colors = FilterChipDefaults.filterChipColors(
                    selectedContainerColor = PrimaryBlue,
                    selectedLabelColor = Color.White
                )
            )

            FilterChip(
                selected = selectedFilter == AppFilter.ACTIVE_ONLY,
                onClick = { selectedFilter = AppFilter.ACTIVE_ONLY },
                label = { Text("${AppStrings.get("filter_active", lang)} (${appsList.count { it.totalSpeedBytes > 0 }})") },
                colors = FilterChipDefaults.filterChipColors(
                    selectedContainerColor = AccentGreen,
                    selectedLabelColor = Color.White
                )
            )

            FilterChip(
                selected = selectedFilter == AppFilter.BLOCKED_ONLY,
                onClick = { selectedFilter = AppFilter.BLOCKED_ONLY },
                label = { Text("${AppStrings.get("filter_blocked", lang)} (${appsList.count { it.isBlocked }})") },
                colors = FilterChipDefaults.filterChipColors(
                    selectedContainerColor = AccentRed,
                    selectedLabelColor = Color.White
                )
            )
        }

        Spacer(modifier = Modifier.height(10.dp))

        // App List
        if (filteredApps.isEmpty()) {
            Box(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(top = 40.dp),
                contentAlignment = Alignment.Center
            ) {
                Text(text = AppStrings.get("no_apps_found", lang), color = TextMuted, fontSize = 13.sp)
            }
        } else {
            LazyColumn(
                verticalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                items(filteredApps, key = { it.packageName }) { app ->
                    AppRowItem(
                        app = app,
                        lang = lang,
                        onBlockToggled = { blocked ->
                            onAppBlockedChanged(app.packageName, blocked)
                        },
                        onOpenSettings = {
                            firewallManager.openAppNetworkSettings(app.packageName)
                        }
                    )
                }
            }
        }
    }
}

@Composable
fun AppRowItem(
    app: AppUsageInfo,
    lang: String = "fa",
    onBlockToggled: (Boolean) -> Unit,
    onOpenSettings: () -> Unit
) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        shape = RoundedCornerShape(12.dp),
        colors = CardDefaults.cardColors(containerColor = SurfaceCard),
        border = BorderStroke(
            1.dp,
            if (app.isBlocked) AccentRed.copy(alpha = 0.5f) else SurfaceCardBorder
        )
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(12.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            // App Icon
            if (app.icon != null) {
                Image(
                    bitmap = app.icon.toBitmap(64, 64).asImageBitmap(),
                    contentDescription = app.appName,
                    modifier = Modifier.size(38.dp)
                )
            } else {
                Box(
                    modifier = Modifier
                        .size(38.dp)
                        .background(PrimaryBlue.copy(alpha = 0.2f), CircleShape),
                    contentAlignment = Alignment.Center
                ) {
                    Text(
                        text = app.appName.take(1).uppercase(),
                        color = AccentSky,
                        fontWeight = FontWeight.Bold,
                        fontSize = 16.sp
                    )
                }
            }

            Spacer(modifier = Modifier.width(12.dp))

            // App Name & Real-time speed / Total data
            Column(modifier = Modifier.weight(1f)) {
                Row(verticalAlignment = Alignment.CenterVertically) {
                    Text(
                        text = app.appName,
                        color = TextPrimary,
                        fontWeight = FontWeight.Bold,
                        fontSize = 14.sp
                    )
                    if (app.isBlocked) {
                        Spacer(modifier = Modifier.width(6.dp))
                        Text(
                            text = AppStrings.get("status_blocked", lang),
                            color = AccentRed,
                            fontWeight = FontWeight.Bold,
                            fontSize = 10.sp
                        )
                    }
                }

                Spacer(modifier = Modifier.height(2.dp))

                if (app.totalSpeedBytes > 0) {
                    Row {
                        Text(
                            text = "↓ ${SpeedFormatter.formatSpeed(app.rxSpeedBytes)}",
                            color = AccentGreen,
                            fontWeight = FontWeight.SemiBold,
                            fontSize = 11.sp
                        )
                        Spacer(modifier = Modifier.width(8.dp))
                        Text(
                            text = "↑ ${SpeedFormatter.formatSpeed(app.txSpeedBytes)}",
                            color = AccentSky,
                            fontWeight = FontWeight.SemiBold,
                            fontSize = 11.sp
                        )
                    }
                } else {
                    Text(
                        text = "${AppStrings.get("total_usage", lang)}: ${SpeedFormatter.formatDataSize(app.todayBytes)}",
                        color = TextMuted,
                        fontSize = 11.sp
                    )
                }
            }

            // Quick App Settings button
            IconButton(
                onClick = onOpenSettings,
                modifier = Modifier.size(32.dp)
            ) {
                Icon(
                    Icons.Default.Settings,
                    contentDescription = "App Settings",
                    tint = TextMuted,
                    modifier = Modifier.size(18.dp)
                )
            }

            Spacer(modifier = Modifier.width(6.dp))

            // Block / Allow Switch
            Switch(
                checked = !app.isBlocked,
                onCheckedChange = { allowed ->
                    onBlockToggled(!allowed)
                },
                colors = SwitchDefaults.colors(
                    checkedTrackColor = AccentGreen,
                    uncheckedTrackColor = AccentRed
                )
            )
        }
    }
}
