package com.netmetr.app.data

import android.graphics.drawable.Drawable

data class AppUsageInfo(
    val packageName: String,
    val appName: String,
    val uid: Int,
    val icon: Drawable? = null,
    val rxSpeedBytes: Double = 0.0,
    val txSpeedBytes: Double = 0.0,
    val todayBytes: Long = 0,
    val isBlocked: Boolean = false,
    val isSystemApp: Boolean = false
) {
    val totalSpeedBytes: Double get() = rxSpeedBytes + txSpeedBytes
}
