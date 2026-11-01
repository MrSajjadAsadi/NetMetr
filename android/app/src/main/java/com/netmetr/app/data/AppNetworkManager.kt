package com.netmetr.app.data

import android.content.Context
import android.content.pm.ApplicationInfo
import android.content.pm.PackageManager
import android.net.TrafficStats
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.isActive
import kotlinx.coroutines.launch

class AppNetworkManager(private val context: Context) {

    private val packageManager: PackageManager = context.packageManager
    private val firewallManager = AppFirewallManager(context)

    private val _appsFlow = MutableStateFlow<List<AppUsageInfo>>(emptyList())
    val appsFlow: StateFlow<List<AppUsageInfo>> = _appsFlow.asStateFlow()

    private var monitorJob: Job? = null
    private val scope = CoroutineScope(Dispatchers.Default)

    private val lastUidStats = mutableMapOf<Int, Pair<Long, Long>>() // uid -> (rx, tx)
    private var lastSampleTime = System.currentTimeMillis()

    fun startMonitoring() {
        monitorJob?.cancel()
        lastSampleTime = System.currentTimeMillis()
        lastUidStats.clear()

        monitorJob = scope.launch {
            while (isActive) {
                sampleApps()
                delay(1500)
            }
        }
    }

    fun stopMonitoring() {
        monitorJob?.cancel()
        monitorJob = null
    }

    private fun sampleApps() {
        val now = System.currentTimeMillis()
        val elapsedSec = (now - lastSampleTime).coerceAtLeast(100) / 1000.0
        val blockedSet = firewallManager.getBlockedPackages()

        val installedApps: List<ApplicationInfo> = try {
            packageManager.getInstalledApplications(PackageManager.GET_META_DATA)
        } catch (e: Exception) {
            emptyList()
        }

        val resultList = mutableListOf<AppUsageInfo>()

        for (app in installedApps) {
            val uid = app.uid
            val rx = TrafficStats.getUidRxBytes(uid)
            val tx = TrafficStats.getUidTxBytes(uid)

            if (rx == TrafficStats.UNSUPPORTED.toLong() || tx == TrafficStats.UNSUPPORTED.toLong()) {
                continue
            }

            // Only consider apps with some cumulative bytes or blocked status
            val prev = lastUidStats[uid]
            var rxSpeed = 0.0
            var txSpeed = 0.0

            if (prev != null) {
                val dRx = (rx - prev.first).coerceAtLeast(0)
                val dTx = (tx - prev.second).coerceAtLeast(0)
                rxSpeed = dRx / elapsedSec
                txSpeed = dTx / elapsedSec
            }

            lastUidStats[uid] = Pair(rx, tx)

            val totalBytes = (rx + tx).coerceAtLeast(0)
            val isBlocked = blockedSet.contains(app.packageName)

            if (totalBytes > 0 || isBlocked) {
                val appName = try {
                    packageManager.getApplicationLabel(app).toString()
                } catch (e: Exception) {
                    app.packageName
                }

                val icon = try {
                    packageManager.getApplicationIcon(app)
                } catch (e: Exception) {
                    null
                }

                val isSystem = (app.flags and ApplicationInfo.FLAG_SYSTEM) != 0

                resultList.add(
                    AppUsageInfo(
                        packageName = app.packageName,
                        appName = appName,
                        uid = uid,
                        icon = icon,
                        rxSpeedBytes = rxSpeed,
                        txSpeedBytes = txSpeed,
                        todayBytes = totalBytes,
                        isBlocked = isBlocked,
                        isSystemApp = isSystem
                    )
                )
            }
        }

        lastSampleTime = now

        // Sort: Active transferring apps first, then highest total usage
        _appsFlow.value = resultList.sortedWith(
            compareByDescending<AppUsageInfo> { it.totalSpeedBytes > 0 }
                .thenByDescending { it.totalSpeedBytes }
                .thenByDescending { it.todayBytes }
        )
    }
}
