package com.netmetr.app.util

import java.util.Locale

object SpeedFormatter {

    fun formatSpeed(bytesPerSec: Double, useBits: Boolean = false): String {
        return if (useBits) {
            val bits = bytesPerSec * 8
            when {
                bits >= 1_000_000_000 -> String.format(Locale.US, "%.2f Gbps", bits / 1_000_000_000.0)
                bits >= 1_000_000 -> String.format(Locale.US, "%.2f Mbps", bits / 1_000_000.0)
                bits >= 1_000 -> String.format(Locale.US, "%.1f Kbps", bits / 1_000.0)
                else -> String.format(Locale.US, "%.0f bps", bits)
            }
        } else {
            when {
                bytesPerSec >= 1_073_741_824 -> String.format(Locale.US, "%.2f GB/s", bytesPerSec / 1_073_741_824.0)
                bytesPerSec >= 1_048_576 -> String.format(Locale.US, "%.2f MB/s", bytesPerSec / 1_048_576.0)
                bytesPerSec >= 1024 -> String.format(Locale.US, "%.1f KB/s", bytesPerSec / 1024.0)
                else -> String.format(Locale.US, "%.0f B/s", bytesPerSec)
            }
        }
    }

    fun formatDataSize(bytes: Long): String {
        val b = bytes.toDouble()
        return when {
            b >= 1_073_741_824.0 * 1024.0 -> String.format(Locale.US, "%.2f TB", b / (1_073_741_824.0 * 1024.0))
            b >= 1_073_741_824.0 -> String.format(Locale.US, "%.2f GB", b / 1_073_741_824.0)
            b >= 1_048_576.0 -> String.format(Locale.US, "%.1f MB", b / 1_048_576.0)
            b >= 1024.0 -> String.format(Locale.US, "%.0f KB", b / 1024.0)
            else -> "$bytes B"
        }
    }
}
