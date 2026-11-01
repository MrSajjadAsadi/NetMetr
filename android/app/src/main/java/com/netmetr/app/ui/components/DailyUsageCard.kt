package com.netmetr.app.ui.components

import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.netmetr.app.data.DailyUsageItem
import com.netmetr.app.ui.theme.AccentGreen
import com.netmetr.app.ui.theme.AccentPurple
import com.netmetr.app.ui.theme.AccentSky
import com.netmetr.app.ui.theme.SurfaceCard
import com.netmetr.app.ui.theme.SurfaceCardBorder
import com.netmetr.app.ui.theme.TextPrimary
import com.netmetr.app.ui.theme.TextSecondary
import com.netmetr.app.util.AppStrings
import com.netmetr.app.util.SpeedFormatter

@Composable
fun DailyUsageCard(
    modifier: Modifier = Modifier,
    usage: DailyUsageItem,
    lang: String = "fa"
) {
    val total = usage.totalBytes
    val wifi = usage.wifiTotal
    val mobile = usage.mobileTotal

    val wifiFraction = if (total > 0) (wifi.toFloat() / total).coerceIn(0f, 1f) else 0.5f
    val mobileFraction = if (total > 0) (mobile.toFloat() / total).coerceIn(0f, 1f) else 0.5f

    Card(
        modifier = modifier,
        shape = RoundedCornerShape(16.dp),
        colors = CardDefaults.cardColors(containerColor = SurfaceCard),
        border = BorderStroke(1.dp, SurfaceCardBorder)
    ) {
        Column(
            modifier = Modifier
                .fillMaxWidth()
                .padding(18.dp)
        ) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text(
                    text = AppStrings.get("today_usage", lang),
                    color = AccentPurple,
                    fontWeight = FontWeight.Bold,
                    fontSize = 12.sp,
                    letterSpacing = 1.sp
                )
                Spacer(modifier = Modifier.weight(1f))
                Text(
                    text = SpeedFormatter.formatDataSize(total),
                    color = TextPrimary,
                    fontWeight = FontWeight.ExtraBold,
                    fontSize = 18.sp
                )
            }

            Spacer(modifier = Modifier.height(14.dp))

            // Two-tone progress bar
            Box(
                modifier = Modifier
                    .fillMaxWidth()
                    .height(8.dp)
                    .clip(RoundedCornerShape(4.dp))
                    .background(SurfaceCardBorder)
            ) {
                Row(modifier = Modifier.fillMaxWidth()) {
                    if (total > 0) {
                        Box(
                            modifier = Modifier
                                .fillMaxHeight()
                                .weight(wifiFraction.coerceAtLeast(0.01f))
                                .background(AccentSky)
                        )
                        Box(
                            modifier = Modifier
                                .fillMaxHeight()
                                .weight(mobileFraction.coerceAtLeast(0.01f))
                                .background(AccentGreen)
                        )
                    }
                }
            }

            Spacer(modifier = Modifier.height(14.dp))

            // Wi-Fi and Mobile breakdown stats
            Row(
                modifier = Modifier.fillMaxWidth()
            ) {
                // Wi-Fi
                Column(modifier = Modifier.weight(1f)) {
                    Row(verticalAlignment = Alignment.CenterVertically) {
                        Box(
                            modifier = Modifier
                                .width(8.dp)
                                .height(8.dp)
                                .clip(RoundedCornerShape(2.dp))
                                .background(AccentSky)
                        )
                        Spacer(modifier = Modifier.width(6.dp))
                        Text(text = AppStrings.get("wifi", lang), color = TextSecondary, fontSize = 12.sp)
                    }
                    Spacer(modifier = Modifier.height(2.dp))
                    Text(
                        text = SpeedFormatter.formatDataSize(wifi),
                        color = TextPrimary,
                        fontWeight = FontWeight.Bold,
                        fontSize = 14.sp
                    )
                }

                // Mobile
                Column(modifier = Modifier.weight(1f)) {
                    Row(verticalAlignment = Alignment.CenterVertically) {
                        Box(
                            modifier = Modifier
                                .width(8.dp)
                                .height(8.dp)
                                .clip(RoundedCornerShape(2.dp))
                                .background(AccentGreen)
                        )
                        Spacer(modifier = Modifier.width(6.dp))
                        Text(text = AppStrings.get("mobile", lang), color = TextSecondary, fontSize = 12.sp)
                    }
                    Spacer(modifier = Modifier.height(2.dp))
                    Text(
                        text = SpeedFormatter.formatDataSize(mobile),
                        color = TextPrimary,
                        fontWeight = FontWeight.Bold,
                        fontSize = 14.sp
                    )
                }
            }
        }
    }
}
