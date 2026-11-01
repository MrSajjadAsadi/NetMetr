package com.netmetr.app.data

import android.content.Context
import android.content.Intent
import android.content.SharedPreferences
import android.net.Uri
import android.provider.Settings
import com.netmetr.app.service.AppFirewallVpnService

class AppFirewallManager(private val context: Context) {

    private val prefs: SharedPreferences = context.getSharedPreferences("netmetr_firewall_prefs", Context.MODE_PRIVATE)

    companion object {
        private const val KEY_BLOCKED_PACKAGES = "blocked_packages"
        private const val KEY_FIREWALL_ENABLED = "firewall_enabled"
    }

    fun getBlockedPackages(): MutableSet<String> {
        val set = prefs.getStringSet(KEY_BLOCKED_PACKAGES, null) ?: emptySet()
        return HashSet(set)
    }

    fun isAppBlocked(packageName: String): Boolean {
        return getBlockedPackages().contains(packageName)
    }

    fun setAppBlocked(packageName: String, blocked: Boolean) {
        val current = getBlockedPackages()
        if (blocked) {
            current.add(packageName)
        } else {
            current.remove(packageName)
        }
        prefs.edit().putStringSet(KEY_BLOCKED_PACKAGES, current).apply()

        // If firewall VPN is running, refresh rules
        if (isFirewallEnabled()) {
            AppFirewallVpnService.startService(context)
        }
    }

    fun isFirewallEnabled(): Boolean {
        return prefs.getBoolean(KEY_FIREWALL_ENABLED, false)
    }

    fun setFirewallEnabled(enabled: Boolean) {
        prefs.edit().putBoolean(KEY_FIREWALL_ENABLED, enabled).apply()
        if (enabled) {
            AppFirewallVpnService.startService(context)
        } else {
            AppFirewallVpnService.stopService(context)
        }
    }

    fun openAppNetworkSettings(packageName: String) {
        try {
            val intent = Intent(Settings.ACTION_APPLICATION_DETAILS_SETTINGS).apply {
                data = Uri.fromParts("package", packageName, null)
                flags = Intent.FLAG_ACTIVITY_NEW_TASK
            }
            context.startActivity(intent)
        } catch (e: Exception) {
            // fallback to general settings
        }
    }
}
