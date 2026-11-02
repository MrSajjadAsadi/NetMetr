package com.netmetr.app.util

object AppStrings {

    fun get(key: String, lang: String = "fa"): String {
        val isFa = lang == "fa"
        return when (key) {
            "app_title" -> if (isFa) "نت‌متر" else "NetMetr"
            "app_subtitle" -> if (isFa) "پایشگر زنده سرعت و حجم مصرفی اینترنت" else "Live Speed & Bandwidth Monitor"
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
            "ethernet" -> if (isFa) "شبکه کابلی" else "Ethernet"
            "disconnected" -> if (isFa) "قطع ارتباط" else "Disconnected"
            "connected" -> if (isFa) "متصل" else "Connected"
            "monitoring_active" -> if (isFa) "پایش اینترنت فعال است" else "Monitoring Active"
            "monitoring_paused" -> if (isFa) "پایش اینترنت متوقف است" else "Monitoring Paused"
            "notif_active_desc" -> if (isFa) "نوتیفیکیشن سرعت در نوار وضعیت نمایش داده می‌شود" else "Notification is displaying live speeds"
            "notif_paused_desc" -> if (isFa) "برای نمایش سرعت در نوار وضعیت روشنش کنید" else "Turn on to see speeds in status bar"
            "realtime_rate" -> if (isFa) "نرخ لحظه‌ای تبادل دیتا" else "Real-time rate"

            // Apps & Firewall
            "apps_title" -> if (isFa) "مدیریت شبکه و فایروال برنامه‌ها" else "App Network & Firewall"
            "apps_subtitle" -> if (isFa) "رصد زنده حجم مصرفی هر برنامه و مسدودسازی فوری دسترسی" else "Real-time per-app bandwidth usage & instant blocking"
            "firewall_active" -> if (isFa) "فایروال برنامه‌ها فعال است" else "Firewall Active"
            "firewall_disabled" -> if (isFa) "فایروال برنامه‌ها غیرفعال است" else "Firewall Disabled"
            "firewall_active_desc" -> if (isFa) "اینترنت برنامه‌های مسدودشده قطع است" else "Blocking network access for selected apps"
            "firewall_disabled_desc" -> if (isFa) "برای مسدودسازی اینترنت برنامه‌ها روشنش کنید" else "Turn on to enforce internet blocks"
            "search_apps" -> if (isFa) "جستجوی برنامه بر اساس نام یا بسته..." else "Search apps by name or package..."
            "filter_all" -> if (isFa) "همه برنامه‌ها" else "All"
            "filter_active" -> if (isFa) "در حال مصرف" else "Active"
            "filter_blocked" -> if (isFa) "مسدودشده‌ها" else "Blocked"
            "status_blocked" -> if (isFa) "مسدود" else "BLOCKED"
            "status_allowed" -> if (isFa) "مجاز" else "ALLOWED"
            "no_apps_found" -> if (isFa) "برنامه‌ای با این مشخصات یافت نشد" else "No applications match your filter"
            "total_usage" -> if (isFa) "مصرف امروز" else "Today's usage"

            // History
            "history_title" -> if (isFa) "سوابق مصرف ترافیک" else "Usage History"
            "history_subtitle" -> if (isFa) "گزارش حجم مصرفی اینترنت طی ۳۰ روز گذشته" else "Data consumption logs over the last 30 days"
            "total_recorded" -> if (isFa) "مجموع مصرف ثبت‌شده" else "TOTAL RECORDED USAGE"
            "days_logged" -> if (isFa) "روز ثبت‌شده" else "Days Logged"
            "no_history" -> if (isFa) "هنوز سابقه‌ای برای نمایش ثبت نشده است" else "No history recorded yet"

            // Settings
            "settings_title" -> if (isFa) "تنظیمات نت‌متر" else "Settings"
            "settings_subtitle" -> if (isFa) "شخصی‌سازی پایش، زبان، واحدها و نوتیفیکیشن" else "Customize monitoring preferences and notifications"
            "lang_section" -> if (isFa) "زبان برنامه (Language)" else "LANGUAGE"
            "lang_persian" -> if (isFa) "فارسی (راست‌چین)" else "Persian (فارسی - RTL)"
            "lang_english" -> if (isFa) "English (Left-to-Right)" else "English (LTR)"
            "display_units_section" -> if (isFa) "نمایش و واحدها" else "DISPLAY & UNITS"
            "use_bits" -> if (isFa) "نمایش با بیت (Mbps / Kbps)" else "Use Bits/sec (Mbps / Kbps)"
            "use_bits_desc_on" -> if (isFa) "نمایش با بیت بر ثانیه (استاندارد اپراتورها)" else "Displaying network bits (Mbps)"
            "use_bits_desc_off" -> if (isFa) "نمایش با بایت بر ثانیه (استاندارد دانلودها - MB/s)" else "Displaying bytes (MB/s)"
            "show_daily_notif" -> if (isFa) "نمایش حجم مصرفی امروز در نوتیفیکیشن" else "Show Daily Usage in Notification"
            "show_daily_notif_desc" -> if (isFa) "نمایش حجم دانلود و آپلود امروز در نوار وضعیت گوشی" else "Display today's downloaded and uploaded amount in status bar"
            "system_section" -> if (isFa) "سیستم و راه‌اندازی خودکار" else "SYSTEM & AUTORUN"
            "start_on_boot" -> if (isFa) "اجرا هنگام روشن شدن گوشی" else "Start on Device Boot"
            "start_on_boot_desc" -> if (isFa) "فعال‌سازی مجدد خودکار پایش پس از ریستارت گوشی" else "Automatically resume monitoring after restarting device"
            "data_mgmt_section" -> if (isFa) "مدیریت داده‌ها" else "DATA MANAGEMENT"
            "reset_today_desc" -> if (isFa) "شمارنده مصرف اینترنت امروز را صفر می‌کند." else "Clear today's measured bandwidth counter back to zero."
            "btn_reset_today" -> if (isFa) "صفر کردن آمار امروز" else "Reset Today's Stats"
            "reset_toast" -> if (isFa) "آمار مصرف امروز صفر شد" else "Today's statistics reset"
            "about_section" -> if (isFa) "درباره برنامه" else "ABOUT"
            "about_desc" -> if (isFa) "نت‌متر v2.0 - پایشگر هوشمند و سبک سرعت و مدیریت مصرف اینترنت با فونت وزیرمتن" else "NetMetr v2.0 - Intelligent speed monitor & bandwidth tracker"

            // Notification
            "notif_today" -> if (isFa) "امروز: %s (وای‌فای: %s • همراه: %s)" else "Today: %s (Wi-Fi: %s • Mobile: %s)"
            "notif_network" -> if (isFa) "شبکه: %s" else "Network: %s"

            else -> key
        }
    }

    fun localizeNetworkType(type: String, lang: String = "fa"): String {
        val isFa = lang == "fa"
        if (!isFa) return type
        return when (type.lowercase()) {
            "wi-fi", "wifi" -> "وای‌فای"
            "mobile data", "mobile", "cellular" -> "اینترنت همراه"
            "ethernet" -> "شبکه کابلی"
            "disconnected" -> "قطع ارتباط"
            "connected" -> "متصل"
            else -> type
        }
    }
}
