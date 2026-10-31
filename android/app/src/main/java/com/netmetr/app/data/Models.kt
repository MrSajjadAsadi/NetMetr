package com.netmetr.app.data

data class SpeedState(
    val rxBytesPerSec: Double = 0.0,
    val txBytesPerSec: Double = 0.0,
    val networkType: String = "Detecting...",
    val isConnected: Boolean = true,
    val isWifi: Boolean = true
)

data class DailyUsageItem(
    val dateKey: String, // "YYYY-MM-DD"
    var wifiRxBytes: Long = 0,
    var wifiTxBytes: Long = 0,
    var mobileRxBytes: Long = 0,
    var mobileTxBytes: Long = 0
) {
    val wifiTotal: Long get() = wifiRxBytes + wifiTxBytes
    val mobileTotal: Long get() = mobileRxBytes + mobileTxBytes
    val totalBytes: Long get() = wifiTotal + mobileTotal
}

data class SettingsData(
    val useBits: Boolean = false,
    val updateIntervalSec: Int = 1,
    val startOnBoot: Boolean = true,
    val showDailyUsageInNotification: Boolean = true,
    val dailyQuotaMb: Long = 0 // 0 means no limit
)
