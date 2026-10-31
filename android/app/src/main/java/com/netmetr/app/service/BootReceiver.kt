package com.netmetr.app.service

import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent
import com.netmetr.app.data.UsageTracker

class BootReceiver : BroadcastReceiver() {

    override fun onReceive(context: Context, intent: Intent) {
        val action = intent.action
        if (action == Intent.ACTION_BOOT_COMPLETED ||
            action == Intent.ACTION_MY_PACKAGE_REPLACED ||
            action == "android.intent.action.QUICKBOOT_POWERON") {

            val tracker = UsageTracker(context)
            val settings = tracker.getSettings()

            if (settings.startOnBoot) {
                NetworkSpeedService.startService(context)
            }
        }
    }
}
