package com.netmetr.app.ui.screens

import android.widget.Toast
import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.Divider
import androidx.compose.material3.FilterChip
import androidx.compose.material3.FilterChipDefaults
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
import com.netmetr.app.data.SettingsData
import com.netmetr.app.ui.theme.AccentRed
import com.netmetr.app.ui.theme.AccentSky
import com.netmetr.app.ui.theme.PrimaryBlue
import com.netmetr.app.ui.theme.SurfaceCard
import com.netmetr.app.ui.theme.SurfaceCardBorder
import com.netmetr.app.ui.theme.TextPrimary
import com.netmetr.app.ui.theme.TextSecondary
import com.netmetr.app.util.AppStrings

@Composable
fun SettingsScreen(
    settings: SettingsData,
    onSettingsChanged: (SettingsData) -> Unit,
    onResetToday: () -> Unit
) {
    val context = LocalContext.current
    val scrollState = rememberScrollState()
    val lang = settings.language

    Column(
        modifier = Modifier
            .fillMaxSize()
            .verticalScroll(scrollState)
            .padding(16.dp)
    ) {
        Text(
            text = AppStrings.get("settings_title", lang),
            color = TextPrimary,
            fontWeight = FontWeight.ExtraBold,
            fontSize = 24.sp
        )
        Text(
            text = AppStrings.get("settings_subtitle", lang),
            color = TextSecondary,
            fontSize = 13.sp
        )

        Spacer(modifier = Modifier.height(16.dp))

        // Language Card (RTL / LTR)
        Card(
            modifier = Modifier.fillMaxWidth(),
            shape = RoundedCornerShape(16.dp),
            colors = CardDefaults.cardColors(containerColor = SurfaceCard),
            border = BorderStroke(1.dp, SurfaceCardBorder)
        ) {
            Column(modifier = Modifier.padding(16.dp)) {
                Text(
                    text = AppStrings.get("lang_section", lang),
                    color = AccentSky,
                    fontWeight = FontWeight.Bold,
                    fontSize = 12.sp,
                    letterSpacing = if (lang == "fa") 0.sp else 1.sp
                )

                Spacer(modifier = Modifier.height(12.dp))

                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.spacedBy(10.dp)
                ) {
                    FilterChip(
                        selected = settings.language == "fa",
                        onClick = { onSettingsChanged(settings.copy(language = "fa")) },
                        label = { Text("فارسی (راست‌چین)") },
                        colors = FilterChipDefaults.filterChipColors(
                            selectedContainerColor = PrimaryBlue,
                            selectedLabelColor = Color.White
                        )
                    )

                    FilterChip(
                        selected = settings.language == "en",
                        onClick = { onSettingsChanged(settings.copy(language = "en")) },
                        label = { Text("English (LTR)") },
                        colors = FilterChipDefaults.filterChipColors(
                            selectedContainerColor = PrimaryBlue,
                            selectedLabelColor = Color.White
                        )
                    )
                }
            }
        }

        Spacer(modifier = Modifier.height(16.dp))

        // Display Settings
        Card(
            modifier = Modifier.fillMaxWidth(),
            shape = RoundedCornerShape(16.dp),
            colors = CardDefaults.cardColors(containerColor = SurfaceCard),
            border = BorderStroke(1.dp, SurfaceCardBorder)
        ) {
            Column(modifier = Modifier.padding(16.dp)) {
                Text(
                    text = AppStrings.get("display_units_section", lang),
                    color = AccentSky,
                    fontWeight = FontWeight.Bold,
                    fontSize = 12.sp,
                    letterSpacing = if (lang == "fa") 0.sp else 1.sp
                )

                Spacer(modifier = Modifier.height(12.dp))

                // Unit toggle
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    verticalAlignment = Alignment.CenterVertically,
                    horizontalArrangement = Arrangement.SpaceBetween
                ) {
                    Column(modifier = Modifier.weight(1f)) {
                        Text(text = AppStrings.get("use_bits", lang), color = TextPrimary, fontWeight = FontWeight.SemiBold, fontSize = 14.sp)
                        Text(
                            text = if (settings.useBits) AppStrings.get("use_bits_desc_on", lang) else AppStrings.get("use_bits_desc_off", lang),
                            color = TextSecondary,
                            fontSize = 12.sp
                        )
                    }
                    Switch(
                        checked = settings.useBits,
                        onCheckedChange = { onSettingsChanged(settings.copy(useBits = it)) },
                        colors = SwitchDefaults.colors(checkedTrackColor = PrimaryBlue)
                    )
                }

                Divider(color = SurfaceCardBorder, modifier = Modifier.padding(vertical = 12.dp))

                // Notification today usage toggle
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    verticalAlignment = Alignment.CenterVertically,
                    horizontalArrangement = Arrangement.SpaceBetween
                ) {
                    Column(modifier = Modifier.weight(1f)) {
                        Text(text = AppStrings.get("show_daily_notif", lang), color = TextPrimary, fontWeight = FontWeight.SemiBold, fontSize = 14.sp)
                        Text(
                            text = AppStrings.get("show_daily_notif_desc", lang),
                            color = TextSecondary,
                            fontSize = 12.sp
                        )
                    }
                    Switch(
                        checked = settings.showDailyUsageInNotification,
                        onCheckedChange = { onSettingsChanged(settings.copy(showDailyUsageInNotification = it)) },
                        colors = SwitchDefaults.colors(checkedTrackColor = PrimaryBlue)
                    )
                }
            }
        }

        Spacer(modifier = Modifier.height(16.dp))

        // System & Service
        Card(
            modifier = Modifier.fillMaxWidth(),
            shape = RoundedCornerShape(16.dp),
            colors = CardDefaults.cardColors(containerColor = SurfaceCard),
            border = BorderStroke(1.dp, SurfaceCardBorder)
        ) {
            Column(modifier = Modifier.padding(16.dp)) {
                Text(
                    text = AppStrings.get("system_section", lang),
                    color = AccentSky,
                    fontWeight = FontWeight.Bold,
                    fontSize = 12.sp,
                    letterSpacing = if (lang == "fa") 0.sp else 1.sp
                )

                Spacer(modifier = Modifier.height(12.dp))

                // Start on boot
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    verticalAlignment = Alignment.CenterVertically,
                    horizontalArrangement = Arrangement.SpaceBetween
                ) {
                    Column(modifier = Modifier.weight(1f)) {
                        Text(text = AppStrings.get("start_on_boot", lang), color = TextPrimary, fontWeight = FontWeight.SemiBold, fontSize = 14.sp)
                        Text(
                            text = AppStrings.get("start_on_boot_desc", lang),
                            color = TextSecondary,
                            fontSize = 12.sp
                        )
                    }
                    Switch(
                        checked = settings.startOnBoot,
                        onCheckedChange = { onSettingsChanged(settings.copy(startOnBoot = it)) },
                        colors = SwitchDefaults.colors(checkedTrackColor = PrimaryBlue)
                    )
                }
            }
        }

        Spacer(modifier = Modifier.height(16.dp))

        // Data Maintenance
        Card(
            modifier = Modifier.fillMaxWidth(),
            shape = RoundedCornerShape(16.dp),
            colors = CardDefaults.cardColors(containerColor = SurfaceCard),
            border = BorderStroke(1.dp, SurfaceCardBorder)
        ) {
            Column(modifier = Modifier.padding(16.dp)) {
                Text(
                    text = AppStrings.get("data_mgmt_section", lang),
                    color = AccentRed,
                    fontWeight = FontWeight.Bold,
                    fontSize = 12.sp,
                    letterSpacing = if (lang == "fa") 0.sp else 1.sp
                )

                Spacer(modifier = Modifier.height(10.dp))

                Text(
                    text = AppStrings.get("reset_today_desc", lang),
                    color = TextSecondary,
                    fontSize = 13.sp
                )

                Spacer(modifier = Modifier.height(12.dp))

                Button(
                    onClick = {
                        onResetToday()
                        Toast.makeText(context, AppStrings.get("reset_toast", lang), Toast.LENGTH_SHORT).show()
                    },
                    colors = ButtonDefaults.buttonColors(containerColor = AccentRed),
                    shape = RoundedCornerShape(8.dp)
                ) {
                    Text(text = AppStrings.get("btn_reset_today", lang), fontWeight = FontWeight.Bold)
                }
            }
        }

        Spacer(modifier = Modifier.height(16.dp))

        // About Card
        Card(
            modifier = Modifier.fillMaxWidth(),
            shape = RoundedCornerShape(16.dp),
            colors = CardDefaults.cardColors(containerColor = SurfaceCard),
            border = BorderStroke(1.dp, SurfaceCardBorder)
        ) {
            Column(modifier = Modifier.padding(16.dp)) {
                Text(
                    text = AppStrings.get("about_section", lang),
                    color = AccentSky,
                    fontWeight = FontWeight.Bold,
                    fontSize = 12.sp,
                    letterSpacing = if (lang == "fa") 0.sp else 1.sp
                )

                Spacer(modifier = Modifier.height(8.dp))

                Text(
                    text = AppStrings.get("about_desc", lang),
                    color = TextSecondary,
                    fontSize = 12.sp,
                    lineHeight = 18.sp
                )
            }
        }

        Spacer(modifier = Modifier.height(24.dp))
    }
}
