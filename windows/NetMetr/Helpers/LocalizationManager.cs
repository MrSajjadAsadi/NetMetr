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
            "Download" => isFa ? "دانلود" : "DOWNLOAD",
            "Upload" => isFa ? "آپلود" : "UPLOAD",
            "Today" => isFa ? "امروز" : "TODAY",
            "Total" => isFa ? "کل" : "Total",
            "CompactMode" => isFa ? "حالت فشرده" : "Compact Mode",
            "ExpandedMode" => isFa ? "حالت گسترده" : "Expanded View",
            "AlwaysOnTop" => isFa ? "همیشه رو به رو" : "Always On Top",
            "LockPosition" => isFa ? "قفل موقعیت (غیرقابل جابجایی)" : "Lock Position (No Drag)",
            "Opacity" => isFa ? "میزان شفافیت" : "Opacity",
            "SpeedUnit" => isFa ? "واحد سرعت" : "Speed Unit",
            "SettingsAndHistory" => isFa ? "تنظیمات و سوابق..." : "Settings & History...",
            "AppManager" => isFa ? "مدیریت و فایروال برنامه‌ها..." : "App Network & Firewall...",
            "ResetToday" => isFa ? "ریست آمار امروز" : "Reset Today's Stats",
            "MinimizeToTray" => isFa ? "کوچک‌سازی به کنار ساعت" : "Minimize to Tray",
            "Exit" => isFa ? "خروج از نت‌متر" : "Exit NetMetr",
            "ShowHideWidget" => isFa ? "نمایش / مخفی‌سازی ویجت" : "Show / Hide Widget",
            "ConfirmReset" => isFa ? "آیا از صفر کردن آمار مصرف امروز مطمئن هستید؟" : "Are you sure you want to reset today's usage statistics?",

            // SettingsWindow
            "SettingsTitle" => isFa ? "نت‌متر - تنظیمات و سوابق مصرف" : "NetMetr - Settings & Usage History",
            "GeneralTab" => isFa ? "⚙ عمومی" : "⚙ General",
            "NetworkTab" => isFa ? "🌐 شبکه" : "🌐 Network",
            "UsageTab" => isFa ? "📊 سوابق مصرف" : "📊 Usage History",
            "AboutTab" => isFa ? "ℹ درباره" : "ℹ About",
            "Language" => isFa ? "زبان برنامه (Language):" : "Application Language:",
            "BehaviorSection" => isFa ? "رفتار پنجره و سیستم" : "Window Behavior",
            "AlwaysOnTopDesc" => isFa ? "نمایش بالاتر از سایر پنجره‌ها (Always on Top)" : "Always on Top (Keep widget above other applications)",
            "LockPosDesc" => isFa ? "قفل کردن موقعیت ویجت روی صفحه" : "Lock Widget Position (Prevent accidental dragging)",
            "SnapEdgesDesc" => isFa ? "چسبیدن خودکار به حاشیه مانیتور هنگام جابجایی" : "Snap to Screen Edges when dragging",
            "StartOnBootDesc" => isFa ? "اجرای خودکار هنگام بالا آمدن ویندوز" : "Start NetMetr automatically with Windows",
            "CompactDefaultDesc" => isFa ? "اجرای اولیه در حالت فشرده (Pill)" : "Launch in Compact Mode by default",
            "AdapterLabel" => isFa ? "کارت شبکه:" : "Network Adapter:",
            "SpeedUnitLabel" => isFa ? "واحد نمایش سرعت:" : "Speed Display Unit:",
            "RefreshRateLabel" => isFa ? "نرخ بروزرسانی:" : "Refresh Interval:",
            "PingSection" => isFa ? "پایشگر پینگ و لتنسی" : "Latency / Ping Monitor",
            "EnablePingDesc" => isFa ? "فعال بودن سنجش پینگ اینترنت" : "Enable Real-Time Ping Monitoring",
            "PingHostLabel" => isFa ? "آدرس سرور پینگ:" : "Ping Host:",
            "SaveApply" => isFa ? "ذخیره و اعمال" : "Save & Apply",
            "Cancel" => isFa ? "انصراف" : "Cancel",

            // AppsWindow
            "AppsTitle" => isFa ? "نت‌متر - مدیریت مصرف و فایروال برنامه‌ها" : "NetMetr - App Network & Firewall Manager",
            "AppsHeader" => isFa ? "برنامه‌های متصل به شبکه و فایروال" : "Active Applications & Network Firewall",
            "AppsSubheader" => isFa ? "مشاهده زنده ترافیک مصرفی برنامه‌ها و قطع فوری دسترسی به اینترنت" : "Real-time per-app bandwidth usage and instant internet blocking",
            "ActiveAppsCount" => isFa ? "برنامه‌های متصل: " : "Active Network Apps: ",
            "SearchPlaceholder" => isFa ? "جستجوی برنامه بر اساس نام یا PID..." : "Search apps by name or PID...",
            "ColApplication" => isFa ? "نرم‌افزار" : "Application",
            "ColDownload" => isFa ? "دانلود" : "Download",
            "ColUpload" => isFa ? "آپلود" : "Upload",
            "ColTotal" => isFa ? "کل مصرف" : "Session Total",
            "ColSockets" => isFa ? "سوکت‌ها" : "Sockets",
            "ColStatus" => isFa ? "وضعیت" : "Status",
            "ColControl" => isFa ? "مدیریت فایروال" : "Firewall Control",
            "StatusActive" => isFa ? "فعال" : "Active",
            "StatusBlocked" => isFa ? "مسدود" : "Blocked",
            "BtnBlock" => isFa ? "قطع نت" : "Block",
            "BtnUnblock" => isFa ? "وصل نت" : "Unblock",
            "BtnEndTask" => isFa ? "بستن برنامه" : "End Task",
            "Close" => isFa ? "بستن" : "Close",
            "FirewallTip" => isFa ? "💡 مسدود کردن یک نرم‌افزار بلافاصله قانونی در فایروال ویندوز ایجاد کرده و مانع از ارسال و دریافت دیتا می‌شود." : "💡 Blocking an application creates an instant Windows Firewall rule blocking all inbound and outbound internet traffic.",

            _ => key
        };
    }
}
