using System.Windows;

namespace NetMetr.Helpers;

public static class LocalizationManager
{
    public static string CurrentLanguage { get; private set; } = "fa"; // "fa" or "en"
    public static FlowDirection CurrentFlowDirection => CurrentLanguage == "fa" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

    public static event Action? OnLanguageChanged;

    public static void SetLanguage(string lang)
    {
        if (lang != "fa" && lang != "en") lang = "fa";
        CurrentLanguage = lang;
        OnLanguageChanged?.Invoke();
    }

    public static string Get(string key)
    {
        bool isFa = CurrentLanguage == "fa";
        return key switch
        {
            // MainWindow
            "AppName" => isFa ? "نت‌متر" : "NetMetr",
            "AppSubtitle" => isFa ? "پایشگر زنده سرعت اینترنت" : "Live Speed Monitor",
            "Download" => isFa ? "دانلود" : "DOWNLOAD",
            "Upload" => isFa ? "آپلود" : "UPLOAD",
            "Today" => isFa ? "امروز" : "TODAY",
            "Total" => isFa ? "مجموع" : "Total",
            "TotalPrefix" => isFa ? "   (مجموع: " : "   (Total: ",
            "Offline" => isFa ? "قطع ارتباط" : "Offline",
            "Online" => isFa ? "متصل" : "Online",
            "Milliseconds" => isFa ? " میلی‌ثانیه" : " ms",
            "CompactMode" => isFa ? "حالت فشرده (Pill)" : "Compact Mode",
            "ExpandedMode" => isFa ? "حالت گسترده" : "Expanded View",
            "AlwaysOnTop" => isFa ? "نمایش همواره رو (Always on Top)" : "Always On Top",
            "LockPosition" => isFa ? "قفل موقعیت (غیرقابل جابجایی)" : "Lock Position (No Drag)",
            "Opacity" => isFa ? "میزان شفافیت" : "Opacity",
            "SpeedUnit" => isFa ? "واحد سرعت" : "Speed Unit",
            "BytesUnit" => isFa ? "بایت بر ثانیه (KB/s, MB/s)" : "Bytes/s (KB/s, MB/s)",
            "BitsUnit" => isFa ? "بیت بر ثانیه (Kbps, Mbps)" : "Bits/s (Kbps, Mbps)",
            "SettingsAndHistory" => isFa ? "تنظیمات و سوابق مصرف..." : "Settings & History...",
            "AppManager" => isFa ? "مدیریت مصرف و فایروال برنامه‌ها..." : "App Network & Firewall...",
            "ResetToday" => isFa ? "صفر کردن آمار امروز" : "Reset Today's Stats",
            "MinimizeToTray" => isFa ? "کوچک‌سازی به کنار ساعت" : "Minimize to Tray",
            "Exit" => isFa ? "خروج از نت‌متر" : "Exit NetMetr",
            "ShowHideWidget" => isFa ? "نمایش / مخفی‌سازی ویجت" : "Show / Hide Widget",
            "ConfirmReset" => isFa ? "آیا از صفر کردن آمار مصرف امروز مطمئن هستید؟" : "Are you sure you want to reset today's usage statistics?",
            "TipExpand" => isFa ? "گسترش ویجت" : "Expand Widget",
            "TipCompact" => isFa ? "حالت فشرده (Pill)" : "Compact Mode",
            "TipSettings" => isFa ? "تنظیمات و سوابق" : "Settings",
            "TipFirewall" => isFa ? "مدیریت مصرف و فایروال برنامه‌ها" : "App Network & Firewall Manager",

            // SettingsWindow
            "SettingsTitle" => isFa ? "نت‌متر - تنظیمات و سوابق مصرف" : "NetMetr - Settings & Usage History",
            "SettingsHeader" => isFa ? "تنظیمات نت‌متر" : "NetMetr Settings",
            "SettingsSubheader" => isFa ? "پایشگر زنده سرعت شبکه و حجم مصرفی اینترنت" : "Real-time Network Monitor & Data Tracker",
            "GeneralTab" => isFa ? "⚙ عمومی" : "⚙ General",
            "NetworkTab" => isFa ? "🌐 شبکه" : "🌐 Network",
            "UsageTab" => isFa ? "📊 سوابق مصرف" : "📊 Usage History",
            "AboutTab" => isFa ? "ℹ درباره" : "ℹ About",
            "Language" => isFa ? "زبان برنامه (Language):" : "Application Language:",
            "BehaviorSection" => isFa ? "رفتار پنجره و سیستم" : "Window Behavior",
            "AlwaysOnTopDesc" => isFa ? "نمایش بالاتر از سایر برنامه‌ها (Always on Top)" : "Always on Top (Keep widget above other applications)",
            "LockPosDesc" => isFa ? "قفل کردن موقعیت ویجت روی صفحه (جلوگیری از جابجایی)" : "Lock Widget Position (Prevent accidental dragging)",
            "SnapEdgesDesc" => isFa ? "چسبیدن هوشمند به لبه‌های مانیتور هنگام جابجایی" : "Snap to Screen Edges when dragging",
            "StartOnBootDesc" => isFa ? "اجرای خودکار همگام با شروع به کار ویندوز" : "Start NetMetr automatically with Windows",
            "CompactDefaultDesc" => isFa ? "اجرای اولیه در حالت فشرده (Pill)" : "Launch in Compact Mode by default",
            "TransparencySection" => isFa ? "میزان شفافیت و نما" : "Transparency & View",
            "OpacityLabel" => isFa ? "میزان شفافیت ویجت:" : "Widget Opacity:",
            "NetworkSettingsSection" => isFa ? "تنظیمات اتصال و کارت شبکه" : "Network Adapter & Interface",
            "AdapterLabel" => isFa ? "کارت شبکه فعال:" : "Network Adapter:",
            "AllAdapters" => isFa ? "تمامی کارت‌های شبکه (پیش‌فرض هوشمند)" : "All Network Interfaces (Default)",
            "SpeedUnitLabel" => isFa ? "واحد نمایش سرعت:" : "Speed Display Unit:",
            "UnitBytesDesc" => isFa ? "بایت بر ثانیه (B/s, KB/s, MB/s) - پیش‌فرض دانلودها" : "Bytes per second (B/s, KB/s, MB/s)",
            "UnitBitsDesc" => isFa ? "بیت بر ثانیه (bps, Kbps, Mbps) - استاندارد اپراتورها" : "Bits per second (bps, Kbps, Mbps)",
            "RefreshRateLabel" => isFa ? "نرخ بروزرسانی سرعت:" : "Refresh Interval:",
            "FastInterval" => isFa ? "۵۰۰ میلی‌ثانیه (بسیار سریع و نرم)" : "500 ms (Fast & Smooth)",
            "NormalInterval" => isFa ? "۱۰۰۰ میلی‌ثانیه (استاندارد - ۱ ثانیه)" : "1000 ms (Normal - 1s)",
            "RelaxedInterval" => isFa ? "۲۰۰۰ میلی‌ثانیه (مصرف باتری کمتر)" : "2000 ms (Relaxed - 2s)",
            "PingSection" => isFa ? "سنجش زنده پینگ و تاخیر شبکه" : "Latency / Ping Monitor",
            "EnablePingDesc" => isFa ? "فعال بودن سنجش زنده پینگ اینترنت" : "Enable Real-Time Ping Monitoring",
            "PingHostLabel" => isFa ? "سرور مقصد پینگ:" : "Ping Host:",
            "HistoryTitle" => isFa ? "سوابق مصرف ترافیک اینترنت" : "Data Consumption History",
            "HistorySubtitle" => isFa ? "مشاهده حجم دانلود و آپلود روزانه ثبت‌شده طی ۳۰ روز گذشته" : "View your daily downloaded and uploaded data over the past 30 days",
            "ColDate" => isFa ? "تاریخ" : "Date",
            "ColTotalUsage" => isFa ? "مجموع مصرف" : "Total Data",
            "ClearHistory" => isFa ? "پاکسازی کل تاریخچه" : "Clear All History",
            "ConfirmClearHistory" => isFa ? "آیا از پاک کردن تمامی سوابق مصرف اطمینان دارید؟" : "Are you sure you want to clear all usage history?",
            "AboutDesc" => isFa ? "نرم‌افزار سبک، مدرن و مینیمال جهت رصد لحظه‌ای سرعت دانلود، آپلود، ترافیک مصرفی روزانه و مدیریت فایروال برنامه‌ها." : "Lightweight, modern real-time internet speed monitor with per-app bandwidth tracking and firewall control.",
            "AboutFramework" => isFa ? "توسعه‌یافته با C# .NET 10 و WPF برای ویندوز، و Jetpack Compose برای اندروید." : "Built with C# .NET 10 WPF & Jetpack Compose.",
            "AboutGithub" => isFa ? "مشاهده مخزن پروژه در گیت‌هاب" : "View on GitHub Repository",
            "SaveApply" => isFa ? "ذخیره و اعمال تنظیمات" : "Save & Apply",
            "Cancel" => isFa ? "انصراف" : "Cancel",

            // AppsWindow
            "AppsTitle" => isFa ? "نت‌متر - مدیریت مصرف و فایروال برنامه‌ها" : "NetMetr - App Network & Firewall Manager",
            "AppsHeader" => isFa ? "برنامه‌های متصل به اینترنت و فایروال" : "Active Applications & Network Firewall",
            "AppsSubheader" => isFa ? "مشاهده زنده ترافیک مصرفی برنامه‌ها و قطع فوری دسترسی به اینترنت با یک کلیک" : "Real-time per-app bandwidth usage and instant internet blocking",
            "ActiveAppsCount" => isFa ? "برنامه‌های متصل: " : "Active Network Apps: ",
            "SearchPlaceholder" => isFa ? "جستجوی برنامه بر اساس نام یا PID..." : "Search apps by name or PID...",
            "FilterAll" => isFa ? "همه برنامه‌ها" : "All Apps",
            "FilterActive" => isFa ? "در حال مصرف دیتا" : "Active Traffic",
            "FilterBlocked" => isFa ? "مسدودشده‌ها" : "Blocked",
            "ColApplication" => isFa ? "نرم‌افزار" : "Application",
            "ColDownload" => isFa ? "دانلود" : "Download",
            "ColUpload" => isFa ? "آپلود" : "Upload",
            "ColTotal" => isFa ? "کل نشست" : "Session Total",
            "ColSockets" => isFa ? "سوکت‌ها" : "Sockets",
            "ColStatus" => isFa ? "وضعیت" : "Status",
            "ColControl" => isFa ? "مدیریت فایروال" : "Firewall Control",
            "StatusActive" => isFa ? "وصل (مجاز)" : "Active",
            "StatusBlocked" => isFa ? "مسدود (قطع)" : "Blocked",
            "BtnBlock" => isFa ? "قطع نت" : "Block",
            "BtnUnblock" => isFa ? "وصل نت" : "Unblock",
            "BtnEndTask" => isFa ? "بستن برنامه" : "End Task",
            "Close" => isFa ? "بستن" : "Close",
            "FirewallTip" => isFa ? "💡 مسدود کردن یک نرم‌افزار بلافاصله قانونی در فایروال ویندوز ایجاد کرده و مانع از ارسال و دریافت دیتا می‌شود." : "💡 Blocking an application creates an instant Windows Firewall rule blocking all inbound and outbound internet traffic.",

            // Tray
            "TrayTip" => isFa ? "نت‌متر - پایشگر زنده سرعت" : "NetMetr - Live Speed Monitor",

            _ => key
        };
    }
}
