package com.netmetr.app.util

object AppStrings {

    fun get(key: String, lang: String = "fa"): String {
        val isFa = lang == "fa"
        return when (key) {
            "app_title" -> if (isFa) "نت‌متر" else "NetMetr"
            "app_subtitle" -> if (isFa) "پایشگر زنده سرعت و حجم اینترنت" else "Live Speed & Bandwidth Monitor"
            "tab_speed" -> if (isFa) "سرعت" else "Speed"
            "tab_apps" -> if (isFa) "برنامه‌ها" else "Apps"
            "tab_history" -> if (isFa) "تاریخچه" else "History"
            "tab_settings" -> if (isFa) "تنظیمات" else "Settings"

            // Dashboard
            "download" -> if (isFa) "دانلود" else "Download"
            "upload" -> if (isFa) "آپلود" else "Upload"
            "today_usage" -> if (isFa) "مصرف امروز اینترنت" else "TODAY'S DATA USAGE"
            "wifi" -> if (isFa) "وای‌فای" else "Wi-Fi"
            "mobile" -> if (isFa) "اینترنت همراه" else "Mobile Data"
            "monitoring_active" -> if (isFa) "پایش اینترنت فعال است" else "Monitoring Active"
            "monitoring_paused" -> if (isFa) "پایش اینترنت متوقف است" else "Monitoring Paused"
            "notif_active_desc" -> if (isFa) "نوتیفیکیشن سرعت در نوار وضعیت نمایش داده می‌شود" else "Notification is displaying live speeds"
            "notif_paused_desc" -> if (isFa) "برای نمایش سرعت در نوار وضعیت روشنش کنید" else "Turn on to see speeds in status bar"
            "realtime_rate" -> if (isFa) "نرخ لحظه‌ای" else "Real-time rate"

            // Apps & Firewall
            "apps_title" -> if (isFa) "مدیریت شبکه و فایروال برنامه‌ها" else "App Network & Firewall"
            "apps_subtitle" -> if (isFa) "رصد حجم مصرفی زنده و مسدودسازی فوری برنامه‌ها" else "Real-time per-app bandwidth usage & instant blocking"
            "firewall_active" -> if (isFa) "فایروال فعال است" else "Firewall Active"
            "firewall_disabled" -> if (isFa) "فایروال غیرفعال است" else "Firewall Disabled"
            "firewall_active_desc" -> if (isFa) "اینترنت برنامه‌های مسدودشده قطع است" else "Blocking network access for selected apps"
            "firewall_disabled_desc" -> if (isFa) "برای مسدودسازی اینترنت برنامه‌ها روشنش کنید" else "Turn on to enforce internet blocks"
            "search_apps" -> if (isFa) "جستجوی برنامه..." else "Search apps..."
            "filter_all" -> if (isFa) "همه" else "All"
            "filter_active" -> if (isFa) "در حال مصرف" else "Active"
            "filter_blocked" -> if (isFa) "مسدود" else "Blocked"
            "status_blocked" -> if (isFa) "مسدود" else "BLOCKED"
            "no_apps_found" -> if (isFa) "برنامه‌ای با این فیلتر یافت نشد" else "No applications match your filter"
            "total_usage" -> if (isFa) "مصرف امروز" else "Today's usage"

            // History
            "history_title" -> if (isFa) "تاریخچه مصرف" else "Usage History"
            "history_subtitle" -> if (isFa) "گزارش مصرف اینترنت طی ۳۰ روز گذشته" else "Data consumption logs over the last 30 days"
            "total_recorded" -> if (isFa) "مجموع مصرف ثبت‌شده" else "TOTAL RECORDED USAGE"
            "days_logged" -> if (isFa) "روز ثبت‌شده" else "Days Logged"
            "no_history" -> if (isFa) "هنوز سوابقی ثبت نشده است" else "No history recorded yet"

            // Settings
            "settings_title" -> if (isFa) "تنظیمات" else "Settings"
            "settings_subtitle" -> if (isFa) "شخصی‌سازی پایش، زبان و نوتیفیکیشن‌ها" else "Customize monitoring preferences and notifications"
            "lang_section" -> if (isFa) "زبان برنامه (Language)" else "LANGUAGE"
            "lang_persian" -> if (isFa) "فارسی (راست‌چین)" else "Persian (فارسی - RTL)"
            "lang_english" -> if (isFa) "English (Left-to-Right)" else "English (LTR)"
            "display_units_section" -> if (isFa) "نمایش و واحدها" else "DISPLAY & UNITS"
            "use_bits" -> if (isFa) "نمایش با بیت (Mbps / Kbps)" else "Use Bits/sec (Mbps / Kbps)"
            "use_bits_desc_on" -> if (isFa) "نمایش با بیت بر ثانیه (Mbps)" else "Displaying network bits (Mbps)"
            "use_bits_desc_off" -> if (isFa) "نمایش با بایت بر ثانیه (MB/s)" else "Displaying bytes (MB/s)"
            "show_daily_notif" -> if (isFa) "نمایش حجم مصرفی امروز در نوتیفیکیشن" else "Show Daily Usage in Notification"
            "show_daily_notif_desc" -> if (isFa) "نمایش مجموع دانلود و آپلود امروز کنار ساعت" else "Display today's downloaded and uploaded amount in status bar"
            "system_section" -> if (isFa) "سیستم و راه‌اندازی خودکار" else "SYSTEM & AUTORUN"
            "start_on_boot" -> if (isFa) "اجرا هنگام روشن شدن گوشی" else "Start on Device Boot"
            "start_on_boot_desc" -> if (isFa) "فعال‌سازی مجدد خودکار پس از ریستارت گوشی" else "Automatically resume monitoring after restarting device"
            "data_mgmt_section" -> if (isFa) "مدیریت داده‌ها" else "DATA MANAGEMENT"
            "reset_today_desc" -> if (isFa) "شمارنده مصرف اینترنت امروز را صفر می‌کند." else "Clear today's measured bandwidth counter back to zero."
            "btn_reset_today" -> if (isFa) "صفر کردن آمار امروز" else "Reset Today's Stats"
            "reset_toast" -> if (isFa) "آمار امروز صفر شد" else "Today's statistics reset"

            else -> key
        }
    }
}
