package com.netmetr.app.data

import android.content.Context
import android.content.SharedPreferences
import com.google.gson.Gson
import com.google.gson.reflect.TypeToken
import java.text.SimpleDateFormat
import java.util.Date
import java.util.Locale

class UsageTracker(context: Context) {

    private val prefs: SharedPreferences = context.getSharedPreferences("netmetr_usage_prefs", Context.MODE_PRIVATE)
    private val gson = Gson()
    private val dateFormat = SimpleDateFormat("yyyy-MM-dd", Locale.US)

    private val lock = Any()
    private var cachedRecords: MutableMap<String, DailyUsageItem> = mutableMapOf()

    init {
        loadRecords()
    }

    private fun getTodayKey(): String = dateFormat.format(Date())

    private fun loadRecords() {
        synchronized(lock) {
            val json = prefs.getString("usage_history_json", null)
            if (!json.isNullOrEmpty()) {
                try {
                    val type = object : TypeToken<MutableMap<String, DailyUsageItem>>() {}.type
                    cachedRecords = gson.fromJson(json, type) ?: mutableMapOf()
                } catch (e: Exception) {
                    cachedRecords = mutableMapOf()
                }
            } else {
                cachedRecords = mutableMapOf()
            }
        }
    }

    private fun saveRecords() {
        synchronized(lock) {
            val json = gson.toJson(cachedRecords)
            prefs.edit().putString("usage_history_json", json).apply()
        }
    }

    fun recordDelta(deltaRx: Long, deltaTx: Long, isMobile: Boolean) {
        if (deltaRx <= 0 && deltaTx <= 0) return

        synchronized(lock) {
            val todayKey = getTodayKey()
            val item = cachedRecords.getOrPut(todayKey) {
                DailyUsageItem(dateKey = todayKey)
            }

            if (isMobile) {
                item.mobileRxBytes += deltaRx
                item.mobileTxBytes += deltaTx
            } else {
                item.wifiRxBytes += deltaRx
                item.wifiTxBytes += deltaTx
            }

            // Save periodically
            saveRecords()
        }
    }

    fun getTodayUsage(): DailyUsageItem {
        synchronized(lock) {
            val todayKey = getTodayKey()
            return cachedRecords[todayKey] ?: DailyUsageItem(dateKey = todayKey)
        }
    }

    fun getHistoryList(days: Int = 30): List<DailyUsageItem> {
        synchronized(lock) {
            return cachedRecords.values
                .sortedByDescending { it.dateKey }
                .take(days)
        }
    }

    fun resetToday() {
        synchronized(lock) {
            val todayKey = getTodayKey()
            cachedRecords.remove(todayKey)
            saveRecords()
        }
    }

    fun getSettings(): SettingsData {
        return SettingsData(
            useBits = prefs.getBoolean("use_bits", false),
            updateIntervalSec = prefs.getInt("update_interval", 1),
            startOnBoot = prefs.getBoolean("start_on_boot", true),
            showDailyUsageInNotification = prefs.getBoolean("show_daily_in_notif", true),
            dailyQuotaMb = prefs.getLong("daily_quota_mb", 0L),
            language = prefs.getString("language", "fa") ?: "fa"
        )
    }

    fun saveSettings(settings: SettingsData) {
        prefs.edit()
            .putBoolean("use_bits", settings.useBits)
            .putInt("update_interval", settings.updateIntervalSec)
            .putBoolean("start_on_boot", settings.startOnBoot)
            .putBoolean("show_daily_in_notif", settings.showDailyUsageInNotification)
            .putLong("daily_quota_mb", settings.dailyQuotaMb)
            .putString("language", settings.language)
            .apply()
    }
}
