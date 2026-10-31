package com.netmetr.app.ui.screens

import androidx.compose.foundation.BorderStroke
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
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.Switch
import androidx.compose.material3.SwitchDefaults
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.netmetr.app.data.DailyUsageItem
import com.netmetr.app.data.SpeedState
import com.netmetr.app.service.NetworkSpeedService
import com.netmetr.app.ui.components.DailyUsageCard
import com.netmetr.app.ui.components.SpeedCard
import com.netmetr.app.ui.theme.AccentGreen
import com.netmetr.app.ui.theme.AccentSky
import com.netmetr.app.ui.theme.PrimaryBlue
import com.netmetr.app.ui.theme.SurfaceCard
import com.netmetr.app.ui.theme.SurfaceCardBorder
import com.netmetr.app.ui.theme.TextMuted
import com.netmetr.app.ui.theme.TextPrimary
import com.netmetr.app.ui.theme.TextSecondary
import com.netmetr.app.util.SpeedFormatter

@Composable
fun DashboardScreen(
    speedState: SpeedState,
    todayUsage: DailyUsageItem,
    isServiceRunning: Boolean,
    useBits: Boolean
) {
    val context = LocalContext.current
    val scrollState = rememberScrollState()

    val downFormatted = SpeedFormatter.formatSpeed(speedState.rxBytesPerSec, useBits)
    val upFormatted = SpeedFormatter.formatSpeed(speedState.txBytesPerSec, useBits)

    Column(
        modifier = Modifier
            .fillMaxSize()
            .verticalScroll(scrollState)
            .padding(16.dp)
    ) {
        // Top Header
        Row(
            modifier = Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Column {
                Text(
                    text = "NetMetr",
                    color = TextPrimary,
                    fontWeight = FontWeight.ExtraBold,
                    fontSize = 26.sp
                )
                Text(
                    text = "Live Speed & Bandwidth Monitor",
                    color = TextSecondary,
                    fontSize = 13.sp
                )
            }

            // Network Type Badge
            Card(
                shape = RoundedCornerShape(20.dp),
                colors = CardDefaults.cardColors(containerColor = SurfaceCard),
                border = BorderStroke(1.dp, SurfaceCardBorder)
            ) {
                Row(
                    modifier = Modifier.padding(horizontal = 12.dp, vertical = 6.dp),
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Box(
                        modifier = Modifier
                            .size(8.dp)
                            .background(if (speedState.isConnected) AccentGreen else Color.Red, CircleShape)
                    )
                    Spacer(modifier = Modifier.width(6.dp))
                    Text(
                        text = speedState.networkType,
                        color = TextPrimary,
                        fontSize = 12.sp,
                        fontWeight = FontWeight.SemiBold
                    )
                }
            }
        }

        Spacer(modifier = Modifier.height(20.dp))

        // Service Master Toggle Card
        Card(
            modifier = Modifier.fillMaxWidth(),
            shape = RoundedCornerShape(16.dp),
            colors = CardDefaults.cardColors(
                containerColor = if (isServiceRunning) PrimaryBlue.copy(alpha = 0.15f) else SurfaceCard
            ),
            border = BorderStroke(
                1.dp,
                if (isServiceRunning) PrimaryBlue.copy(alpha = 0.5f) else SurfaceCardBorder
            )
        ) {
            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(16.dp),
                verticalAlignment = Alignment.CenterVertically,
                horizontalArrangement = Arrangement.SpaceBetween
            ) {
                Column(modifier = Modifier.weight(1f)) {
                    Text(
                        text = if (isServiceRunning) "Monitoring Active" else "Monitoring Paused",
                        color = TextPrimary,
                        fontWeight = FontWeight.Bold,
                        fontSize = 16.sp
                    )
                    Spacer(modifier = Modifier.height(2.dp))
                    Text(
                        text = if (isServiceRunning)
                            "Notification is displaying live speeds"
                        else
                            "Turn on to see speeds in status bar",
                        color = TextSecondary,
                        fontSize = 12.sp
                    )
                }

                Switch(
                    checked = isServiceRunning,
                    onCheckedChange = { start ->
                        if (start) {
                            NetworkSpeedService.startService(context)
                        } else {
                            NetworkSpeedService.stopService(context)
                        }
                    },
                    colors = SwitchDefaults.colors(
                        checkedThumbColor = Color.White,
                        checkedTrackColor = PrimaryBlue,
                        uncheckedThumbColor = TextMuted,
                        uncheckedTrackColor = SurfaceCardBorder
                    )
                )
            }
        }

        Spacer(modifier = Modifier.height(16.dp))

        // Real-Time Speed Cards (Download & Upload)
        Row(
            modifier = Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.spacedBy(12.dp)
        ) {
            SpeedCard(
                modifier = Modifier.weight(1f),
                title = "Download",
                speedFormatted = downFormatted,
                accentColor = AccentGreen,
                iconSymbol = "↓"
            )

            SpeedCard(
                modifier = Modifier.weight(1f),
                title = "Upload",
                speedFormatted = upFormatted,
                accentColor = AccentSky,
                iconSymbol = "↑"
            )
        }

        Spacer(modifier = Modifier.height(16.dp))

        // Today's Usage Breakdown
        DailyUsageCard(
            modifier = Modifier.fillMaxWidth(),
            usage = todayUsage
        )

        Spacer(modifier = Modifier.height(16.dp))

        // Session Details Card
        Card(
            modifier = Modifier.fillMaxWidth(),
            shape = RoundedCornerShape(16.dp),
            colors = CardDefaults.cardColors(containerColor = SurfaceCard),
            border = BorderStroke(1.dp, SurfaceCardBorder)
        ) {
            Column(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(16.dp)
            ) {
                Text(
                    text = "STATUS BAR NOTIFICATION",
                    color = AccentSky,
                    fontWeight = FontWeight.Bold,
                    fontSize = 12.sp,
                    letterSpacing = 1.sp
                )
                Spacer(modifier = Modifier.height(8.dp))
                Text(
                    text = "• Updates continuously every second in your notification shade.\n• Displays live download and upload traffic alongside today's consumption.\n• Starts automatically on device restart if enabled in Settings.",
                    color = TextSecondary,
                    fontSize = 13.sp,
                    lineHeight = 20.sp
                )
            }
        }

        Spacer(modifier = Modifier.height(24.dp))
    }
}
