package com.netmetr.app.service

import android.app.Notification
import android.app.PendingIntent
import android.app.Service
import android.content.Context
import android.content.Intent
import android.content.pm.ServiceInfo
import android.net.ConnectivityManager
import android.net.NetworkCapabilities
import android.net.TrafficStats
import android.os.Build
import android.os.IBinder
import androidx.core.app.NotificationCompat
import com.netmetr.app.MainActivity
import com.netmetr.app.NetMetrApp
import com.netmetr.app.R
import com.netmetr.app.data.DailyUsageItem
import com.netmetr.app.data.SpeedState
import com.netmetr.app.data.UsageTracker
import com.netmetr.app.util.SpeedFormatter
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.isActive
import kotlinx.coroutines.launch

class NetworkSpeedService : Service() {

    companion object {
        const val NOTIFICATION_ID = 1001
        const val ACTION_START = "com.netmetr.app.action.START"
        const val ACTION_STOP = "com.netmetr.app.action.STOP"

        private val _speedFlow = MutableStateFlow(SpeedState())
        val speedFlow: StateFlow<SpeedState> = _speedFlow.asStateFlow()

        private val _todayUsageFlow = MutableStateFlow(DailyUsageItem("today"))
        val todayUsageFlow: StateFlow<DailyUsageItem> = _todayUsageFlow.asStateFlow()

        private val _isServiceRunning = MutableStateFlow(false)
        val isServiceRunning: StateFlow<Boolean> = _isServiceRunning.asStateFlow()

        fun startService(context: Context) {
            val intent = Intent(context, NetworkSpeedService::class.java).apply {
                action = ACTION_START
            }
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                context.startForegroundService(intent)
            } else {
                context.startService(intent)
            }
        }

        fun stopService(context: Context) {
            val intent = Intent(context, NetworkSpeedService::class.java).apply {
                action = ACTION_STOP
            }
            context.startService(intent)
        }
    }

    private var monitorJob: Job? = null
    private val scope = CoroutineScope(Dispatchers.Default)

    private lateinit var usageTracker: UsageTracker
    private lateinit var connectivityManager: ConnectivityManager

    private var lastTotalRx: Long = 0
    private var lastTotalTx: Long = 0
    private var lastTimestamp: Long = 0
    private var isFirstSample: Boolean = true

    override fun onCreate() {
        super.onCreate()
        usageTracker = UsageTracker(this)
        connectivityManager = getSystemService(Context.CONNECTIVITY_SERVICE) as ConnectivityManager
    }

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        if (intent?.action == ACTION_STOP) {
            stopMonitoring()
            stopForeground(STOP_FOREGROUND_REMOVE)
            stopSelf()
            return START_NOT_STICKY
        }

        startForegroundServiceNotification()
        startMonitoring()
        return START_STICKY
    }

    private fun startForegroundServiceNotification() {
        val notification = buildNotification("Initializing...", "Tracking real-time network traffic")
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.UPSIDE_DOWN_CAKE) {
            startForeground(
                NOTIFICATION_ID,
                notification,
                ServiceInfo.FOREGROUND_SERVICE_TYPE_SPECIAL_USE
            )
        } else {
            startForeground(NOTIFICATION_ID, notification)
        }
        _isServiceRunning.value = true
    }

    private fun startMonitoring() {
        monitorJob?.cancel()
        isFirstSample = true

        val settings = usageTracker.getSettings()
        val intervalMs = (settings.updateIntervalSec.coerceAtLeast(1)) * 1000L

        monitorJob = scope.launch {
            while (isActive) {
                sampleAndNotify()
                delay(intervalMs)
            }
        }
    }

    private fun stopMonitoring() {
        monitorJob?.cancel()
        monitorJob = null
        _isServiceRunning.value = false
    }

    private fun sampleAndNotify() {
        val now = System.currentTimeMillis()
        val currentRx = TrafficStats.getTotalRxBytes()
        val currentTx = TrafficStats.getTotalTxBytes()

        if (currentRx == TrafficStats.UNSUPPORTED.toLong() || currentTx == TrafficStats.UNSUPPORTED.toLong()) {
            return
        }

        if (isFirstSample) {
            lastTotalRx = currentRx
            lastTotalTx = currentTx
            lastTimestamp = now
            isFirstSample = false
            return
        }

        val elapsedMillis = (now - lastTimestamp).coerceAtLeast(100)
        val elapsedSec = elapsedMillis / 1000.0

        val deltaRx = (currentRx - lastTotalRx).coerceAtLeast(0)
        val deltaTx = (currentTx - lastTotalTx).coerceAtLeast(0)

        lastTotalRx = currentRx
        lastTotalTx = currentTx
        lastTimestamp = now

        val rxSpeed = deltaRx / elapsedSec
        val txSpeed = deltaTx / elapsedSec

        // Network type check
        val networkInfo = getNetworkType()
        val isMobile = !networkInfo.isWifi

        // Record usage
        usageTracker.recordDelta(deltaRx, deltaTx, isMobile)

        val settings = usageTracker.getSettings()
        val todayItem = usageTracker.getTodayUsage()
        _todayUsageFlow.value = todayItem

        val speedState = SpeedState(
            rxBytesPerSec = rxSpeed,
            txBytesPerSec = txSpeed,
            networkType = networkInfo.typeName,
            isConnected = networkInfo.isConnected,
            isWifi = networkInfo.isWifi
        )
        _speedFlow.value = speedState

        // Update Android Status Bar & Notification
        val downFormatted = SpeedFormatter.formatSpeed(rxSpeed, settings.useBits)
        val upFormatted = SpeedFormatter.formatSpeed(txSpeed, settings.useBits)
        val title = "↓ $downFormatted   ↑ $upFormatted"

        val body = if (settings.showDailyUsageInNotification) {
            val totalFormatted = SpeedFormatter.formatDataSize(todayItem.totalBytes)
            val wifiFormatted = SpeedFormatter.formatDataSize(todayItem.wifiTotal)
            val mobileFormatted = SpeedFormatter.formatDataSize(todayItem.mobileTotal)
            "Today: $totalFormatted (Wi-Fi: $wifiFormatted • Mobile: $mobileFormatted)"
        } else {
            "Network: ${networkInfo.typeName}"
        }

        val notification = buildNotification(title, body)
        val manager = getSystemService(Context.NOTIFICATION_SERVICE) as android.app.NotificationManager
        manager.notify(NOTIFICATION_ID, notification)
    }

    private data class NetworkTypeInfo(val typeName: String, val isConnected: Boolean, val isWifi: Boolean)

    private fun getNetworkType(): NetworkTypeInfo {
        val activeNetwork = connectivityManager.activeNetwork ?: return NetworkTypeInfo("Disconnected", false, false)
        val caps = connectivityManager.getNetworkCapabilities(activeNetwork) ?: return NetworkTypeInfo("Disconnected", false, false)

        return when {
            caps.hasTransport(NetworkCapabilities.TRANSPORT_WIFI) -> NetworkTypeInfo("Wi-Fi", true, true)
            caps.hasTransport(NetworkCapabilities.TRANSPORT_CELLULAR) -> NetworkTypeInfo("Mobile Data", true, false)
            caps.hasTransport(NetworkCapabilities.TRANSPORT_ETHERNET) -> NetworkTypeInfo("Ethernet", true, true)
            else -> NetworkTypeInfo("Connected", true, false)
        }
    }

    private fun buildNotification(title: String, content: String): Notification {
        val openAppIntent = Intent(this, MainActivity::class.java).apply {
            flags = Intent.FLAG_ACTIVITY_SINGLE_TOP or Intent.FLAG_ACTIVITY_CLEAR_TOP
        }
        val openPendingIntent = PendingIntent.getActivity(
            this,
            0,
            openAppIntent,
            PendingIntent.FLAG_UPDATE_CURRENT or PendingIntent.FLAG_IMMUTABLE
        )

        val stopIntent = Intent(this, NetworkSpeedService::class.java).apply {
            action = ACTION_STOP
        }
        val stopPendingIntent = PendingIntent.getService(
            this,
            1,
            stopIntent,
            PendingIntent.FLAG_UPDATE_CURRENT or PendingIntent.FLAG_IMMUTABLE
        )

        return NotificationCompat.Builder(this, NetMetrApp.CHANNEL_ID)
            .setSmallIcon(R.drawable.ic_stat_speed)
            .setContentTitle(title)
            .setContentText(content)
            .setOngoing(true)
            .setOnlyAlertOnce(true)
            .setContentIntent(openPendingIntent)
            .addAction(R.drawable.ic_stat_speed, getString(R.string.action_stop), stopPendingIntent)
            .setPriority(NotificationCompat.PRIORITY_LOW)
            .setCategory(NotificationCompat.CATEGORY_SERVICE)
            .setVisibility(NotificationCompat.VISIBILITY_PUBLIC)
            .build()
    }

    override fun onDestroy() {
        stopMonitoring()
        super.onDestroy()
    }

    override fun onBind(intent: Intent?): IBinder? = null
}
