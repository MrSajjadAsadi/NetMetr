package com.netmetr.app.ui.screens

import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.netmetr.app.data.DailyUsageItem
import com.netmetr.app.ui.theme.AccentGreen
import com.netmetr.app.ui.theme.AccentSky
import com.netmetr.app.ui.theme.SurfaceCard
import com.netmetr.app.ui.theme.SurfaceCardBorder
import com.netmetr.app.ui.theme.TextMuted
import com.netmetr.app.ui.theme.TextPrimary
import com.netmetr.app.ui.theme.TextSecondary
import com.netmetr.app.util.AppStrings
import com.netmetr.app.util.SpeedFormatter

@Composable
fun HistoryScreen(
    historyList: List<DailyUsageItem>,
    lang: String = "fa"
) {
    val totalHistoryBytes = historyList.sumOf { it.totalBytes }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(16.dp)
    ) {
        Text(
            text = AppStrings.get("history_title", lang),
            color = TextPrimary,
            fontWeight = FontWeight.ExtraBold,
            fontSize = 24.sp
        )
        Text(
            text = AppStrings.get("history_subtitle", lang),
            color = TextSecondary,
            fontSize = 13.sp
        )

        Spacer(modifier = Modifier.height(16.dp))

        // Total Cumulative Card
        Card(
            modifier = Modifier.fillMaxWidth(),
            shape = RoundedCornerShape(16.dp),
            colors = CardDefaults.cardColors(containerColor = SurfaceCard),
            border = BorderStroke(1.dp, SurfaceCardBorder)
        ) {
            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(16.dp),
                verticalAlignment = Alignment.CenterVertically,
                horizontalArrangement = Arrangement.SpaceBetween
            ) {
                Column {
                    Text(text = AppStrings.get("total_recorded", lang), color = TextMuted, fontSize = 11.sp, fontWeight = FontWeight.Bold)
                    Spacer(modifier = Modifier.height(2.dp))
                    Text(
                        text = SpeedFormatter.formatDataSize(totalHistoryBytes),
                        color = TextPrimary,
                        fontWeight = FontWeight.ExtraBold,
                        fontSize = 22.sp
                    )
                }

                Text(
                    text = "${historyList.size} ${AppStrings.get("days_logged", lang)}",
                    color = AccentSky,
                    fontWeight = FontWeight.SemiBold,
                    fontSize = 12.sp
                )
            }
        }

        Spacer(modifier = Modifier.height(16.dp))

        if (historyList.isEmpty()) {
            Column(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(top = 40.dp),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                Text(text = AppStrings.get("no_history", lang), color = TextMuted, fontSize = 14.sp)
            }
        } else {
            LazyColumn(
                verticalArrangement = Arrangement.spacedBy(10.dp)
            ) {
                items(historyList) { item ->
                    Card(
                        modifier = Modifier.fillMaxWidth(),
                        shape = RoundedCornerShape(12.dp),
                        colors = CardDefaults.cardColors(containerColor = SurfaceCard),
                        border = BorderStroke(1.dp, SurfaceCardBorder)
                    ) {
                        Row(
                            modifier = Modifier
                                .fillMaxWidth()
                                .padding(14.dp),
                            verticalAlignment = Alignment.CenterVertically,
                            horizontalArrangement = Arrangement.SpaceBetween
                        ) {
                            Column {
                                Text(
                                    text = item.dateKey,
                                    color = TextPrimary,
                                    fontWeight = FontWeight.Bold,
                                    fontSize = 14.sp
                                )
                                Spacer(modifier = Modifier.height(4.dp))
                                Row {
                                    Text(
                                        text = "${AppStrings.get("wifi", lang)}: ${SpeedFormatter.formatDataSize(item.wifiTotal)}",
                                        color = AccentSky,
                                        fontSize = 11.sp
                                    )
                                    Spacer(modifier = Modifier.padding(horizontal = 4.dp))
                                    Text(
                                        text = "•  ${AppStrings.get("mobile", lang)}: ${SpeedFormatter.formatDataSize(item.mobileTotal)}",
                                        color = AccentGreen,
                                        fontSize = 11.sp
                                    )
                                }
                            }

                            Text(
                                text = SpeedFormatter.formatDataSize(item.totalBytes),
                                color = TextPrimary,
                                fontWeight = FontWeight.ExtraBold,
                                fontSize = 16.sp
                            )
                        }
                    }
                }
            }
        }
    }
}
