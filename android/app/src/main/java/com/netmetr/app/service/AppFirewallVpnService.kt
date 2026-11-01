package com.netmetr.app.service

import android.content.Context
import android.content.Intent
import android.net.VpnService
import android.os.ParcelFileDescriptor
import com.netmetr.app.data.AppFirewallManager
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.isActive
import kotlinx.coroutines.launch
import java.io.FileInputStream
import java.io.FileOutputStream

class AppFirewallVpnService : VpnService() {

    companion object {
        const val ACTION_START = "com.netmetr.app.firewall.START"
        const val ACTION_STOP = "com.netmetr.app.firewall.STOP"

        fun startService(context: Context) {
            val intent = Intent(context, AppFirewallVpnService::class.java).apply {
                action = ACTION_START
            }
            context.startService(intent)
        }

        fun stopService(context: Context) {
            val intent = Intent(context, AppFirewallVpnService::class.java).apply {
                action = ACTION_STOP
            }
            context.startService(intent)
        }
    }

    private var vpnInterface: ParcelFileDescriptor? = null
    private var vpnJob: Job? = null
    private val scope = CoroutineScope(Dispatchers.IO)

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        if (intent?.action == ACTION_STOP) {
            stopVpn()
            stopSelf()
            return START_NOT_STICKY
        }

        setupFirewallTunnel()
        return START_STICKY
    }

    private fun setupFirewallTunnel() {
        stopVpn()

        val firewallManager = AppFirewallManager(this)
        val blockedPackages = firewallManager.getBlockedPackages()

        if (blockedPackages.isEmpty()) {
            // Nothing to block
            return
        }

        try {
            val builder = Builder()
                .setSession("NetMetr Firewall")
                .addAddress("10.254.1.1", 30)
                .addRoute("0.0.0.0", 0)

            // Only capture traffic from BLOCKED apps into our blackhole tunnel.
            // All unblocked apps completely bypass this VPN and enjoy full direct speed!
            var addedCount = 0
            for (pkg in blockedPackages) {
                try {
                    builder.addAllowedApplication(pkg)
                    addedCount++
                } catch (e: Exception) {
                    // App might have been uninstalled
                }
            }

            if (addedCount == 0) return

            vpnInterface = builder.establish()

            // Run blackhole reader loop to sink packets from blocked apps
            vpnInterface?.let { pfd ->
                vpnJob = scope.launch {
                    val input = FileInputStream(pfd.fileDescriptor)
                    val buffer = ByteArray(32768)
                    try {
                        while (isActive) {
                            val read = input.read(buffer)
                            if (read <= 0) break
                            // Dropping packet - blocked app has no internet!
                        }
                    } catch (e: Exception) {
                        // socket closed
                    }
                }
            }
        } catch (e: Exception) {
            // VPN permission not granted or service interrupted
        }
    }

    private fun stopVpn() {
        vpnJob?.cancel()
        vpnJob = null
        try {
            vpnInterface?.close()
        } catch (e: Exception) {
            // ignore
        }
        vpnInterface = null
    }

    override fun onDestroy() {
        stopVpn()
        super.onDestroy()
    }
}
