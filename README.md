# ⚡ NetMetr — Real-Time Network Speed Meter & Bandwidth Tracker
### اپلیکیشن نمایش لحظه‌ای سرعت آپلود، دانلود و حجم مصرفی اینترنت (ویندوز و اندروید)

<div align="center">

![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Android-blue?style=for-the-badge)
![GitHub Actions](https://img.shields.io/badge/CI%2FCD-GitHub%20Actions%20Ready-2ea44f?style=for-the-badge&logo=githubactions)
![UI](https://img.shields.io/badge/UI-Jetpack%20Compose%20%7C%20WPF%20Glass-8a2be2?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

</div>

---

## 🌟 معرفی پروژه (Project Overview)

**NetMetr** یک نرم‌افزار سبک، مدرن، بسیار دقیق و زیبا برای نظارت لحظه‌ای بر ترافیک اینترنت است که برای دو پلتفرم **ویندوز (Windows)** و **اندروید (Android)** طراحی و پیاده‌سازی شده است.

---

## 💻 ۱. نسخه ویندوز (Windows Desktop Overlay)

نسخه ویندوز یک **ویجت شناور (Floating Widget)** مدرن با استایل شیشه‌ای (Glassmorphism / Dark Acrylic) است که همیشه روی سایر برنامه‌ها قرار می‌گیرد و به راحتی با ماوس روی صفحه جابجا می‌شود.

### 🚀 قابلیت‌های برجسته نسخه ویندوز:
- 🪟 **جابجایی آزاد روی تمام برنامه‌ها (Draggable & Always on Top):** با نگه داشتن کلیک چپ ماوس، ویجت را به هر نقطه‌ای از مانیتور منتقل کنید.
- 🧲 **قابلیت چسبیدن خودکار به لبه‌های تصویر (Magnetic Snap-to-Edge):** هنگام نزدیک شدن به لبه‌های مانیتور یا تسک‌بار، به نرمی می‌چسبد.
- 📏 **دو حالت نمایش فشرده و پیشرفته (Compact vs Expanded):**
  - **حالت فشرده (Compact Mode):** یک کپسول باریک و مینیمال شامل سرعت دانلود، آپلود و پینگ.
  - **حالت پیشرفته (Expanded Mode):** نمایش گراف لحظه‌ای، پینگ، کارت‌های مجزا، حجم مصرفی امروز و نام کارت شبکه.
- 📈 **نمودار زنده‌ی تاریخچه سرعت (Real-time Sparkline Graph):** رسم زنده منحنی دانلود و آپلود ۳۵ ثانیه گذشته.
- 🟢 **نمایش پینگ لحظه‌ای (Live Ping / Latency Monitor):** سنجش خودکار پینگ (مثلاً به `8.8.8.8` یا سرور دلخواه) با تغییر رنگ شاخص (سبز، زرد، قرمز).
- 📊 **محاسبه و ذخیره حجم مصرفی امروز (Daily Bandwidth Tracker):** تفکیک حجم دانلود و آپلود امروز + بایگانی تاریخچه تا ۳۰ روز گذشته در دیتابیس JSON محلی.
- 🔒 **حالت قفل موقعیت (Lock Position):** برای جلوگیری از جابجایی تصادفی در بازی‌ها یا حین کار.
- 🎛 **تنظیم میزان شفافیت (Opacity Control):** از ۴۰٪ تا ۱۰۰٪ از طریق کلیک راست یا پنجره تنظیمات.
- 🔔 **آیکون تسک‌بار و منوی Tray (System Tray Integration):** نمایش سرعت زنده در تول‌تیپ آیکون کنار ساعت و امکان مخفی‌سازی/نمایش مجدد.
- ⚡ **بسیار سبک و بهینه:** مصرف پردازنده نزدیک به صفر (کمتر از ۰.۱٪ CPU) بدون افت فریم در بازی‌ها.

---

## 📱 ۲. نسخه اندروید (Android Companion App)

نسخه اندروید با معماری مدرن **Kotlin + Jetpack Compose (Material 3)** توسعه یافته و با سرویس پیش‌زمینه (Foreground Service) و نوتیفیکیشن دائم کار می‌کند.

### 🚀 قابلیت‌های برجسته نسخه اندروید:
- 📌 **نوتیفیکیشن دائم با سرعت لحظه‌ای و حجم مصرفی امروز:**
  - عنوان نوتیفیکیشن: `↓ 4.8 MB/s   ↑ 512 KB/s`
  - متن نوتیفیکیشن: `Today: 1.2 GB (Wi-Fi: 1.1 GB • Mobile: 100 MB)`
  - همیشه در نوار وضعیت (Status Bar) و صفحه قفل (Lock Screen) بدون ایجاد صدا یا لرزش مزاحم قرار دارد.
- 🔄 **اجرای خودکار پس از روشن شدن گوشی (Start on Boot):** دارای `BootReceiver` برای فعال‌سازی خودکار پایش ترافیک بعد از ریستارت گوشی.
- 📶 **تفکیک هوشمند مصرف Wi-Fi و اینترنت سیم‌کارت (Mobile Data):** بررسی لحظه‌ای نوع اتصال و ثبت مجزای هرکدام.
- 📊 **داشبورد جذاب با Jetpack Compose و تم دارک متریال ۳:**
  - گیج‌ها و کارت‌های رنگی سرعت آپلود و دانلود
  - نمودار نواری دو رنگ برای تفکیک درصد مصرف روزانه وای‌فای و دیتا
  - تب تاریخچه (History) با گزارش مصرف روزانه تا ۳۰ روز گذشته
  - تب تنظیمات (Settings) برای تغییر واحد به بیت/بایت (MB/s یا Mbps)، زمان بروزرسانی و ریست آمار.
- 🔋 **بهینه‌سازی حداکثری باتری:** استفاده از API نیتیو `TrafficStats` سیستم‌عامل اندروید بدون هرگونه مصرف اضافه باتری.

---

## ⚙️ ۳. نحوه کامپایل خودکار با GitHub Actions (CI/CD)

کاربر گرامی، پروژه طوری پیکربندی شده است که **بدون نیاز به نصب اندروید استودیو روی سیستم**، با قرار دادن کد در گیت‌هاب، اکشن‌های آماده به صورت اتوماتیک فایل‌های نصبی را می‌سازند:

1. پوشه `.github/workflows/android.yml`:
   - کد را تحویل می‌گیرد.
   - جاوا ۱۷ و گریدل ۸.۵ را کانفیگ می‌کند.
   - دستور `./gradlew assembleDebug` و `./gradlew assembleRelease` را اجرا می‌کند.
   - فایل نهایی **`app-debug.apk`** را در تب **Artifacts** گیت‌هاب قرار می‌دهد تا مستقیماً روی گوشی نصب کنید!

2. پوشه `.github/workflows/windows.yml`:
   - نسخه ویندوز را کامپایل کرده و یک فایل تک‌فایلی **`NetMetr.exe`** مستقل و بدون نیاز به نصب تولید می‌کند.

### راهنمای قرار دادن در گیت‌هاب (Push to GitHub):
```bash
# ۱. اتصال به ریپازیتوری گیت‌هاب خود:
git add .
git commit -m "Initial commit of NetMetr Windows and Android"
git branch -M main
git remote add origin https://github.com/<YOUR_USERNAME>/<YOUR_REPO_NAME>.git
git push -u origin main
```
پس از ارسال (Push)، به تب **Actions** در صفحه گیت‌هاب بروید؛ خواهید دید که ساخت APK و EXE به شکل خودکار آغاز شده و پس از پایان ساخت، فایل نصبی آماده دانلود است.

---

## 🛠 ساخت و اجرای محلی (Local Development)

### اجرای نسخه ویندوز:
```powershell
# ورود به پوشه و کامپایل
cd windows/NetMetr
dotnet run
```
یا ساخت فایل نصبی تک‌فایلی پرسرعت:
```powershell
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o ../publish/
```

### ساخت محلی نسخه اندروید (در صورت داشتن Android SDK / Android Studio):
```bash
cd android
./gradlew assembleDebug
```
فایل APK در مسیر زیر تولید می‌شود:
`android/app/build/outputs/apk/debug/app-debug.apk`

---

## 📁 ساختار پوشه‌های پروژه (Folder Structure)

```
NetMetr/
├── .github/
│   └── workflows/
│       ├── android.yml              # اکشن کامپایل خودکار APK اندروید
│       └── windows.yml              # اکشن ساخت EXE ویندوز
├── windows/
│   └── NetMetr/
│       ├── App.xaml / App.xaml.cs
│       ├── MainWindow.xaml / .cs    # پنجره شناور شیشه‌ای و درگبل روی دسکتاپ
│       ├── SettingsWindow.xaml / .cs# پنجره تنظیمات و جدول تاریخچه ۳۰ روزه
│       ├── Models/                  # مدل‌های سرعت، حجم روزانه و تنظیمات
│       ├── Services/
│       │   ├── NetworkMonitorService.cs # موتور محاسبه دلتای بایت‌های دریافتی/ارسالی
│       │   ├── PingService.cs           # پایشگر پینگ غیرهمگام
│       │   ├── UsageTrackerService.cs   # ذخیره تاریخچه مصرف روزانه
│       │   └── TrayService.cs           # آیکون تسک‌بار ویندوز
│       └── Helpers/
│           ├── Formatter.cs             # فرمت هوشمند سرعت و حجم (MB/s, Mbps, ...)
│           └── AutoStartHelper.cs       # اجرای خودکار همراه با ویندوز
└── android/
    ├── gradlew / gradlew.bat        # ابزار Gradle Wrapper
    ├── settings.gradle.kts
    ├── build.gradle.kts
    └── app/
        ├── build.gradle.kts         # وابستگی‌های Jetpack Compose و Material 3
        └── src/main/
            ├── AndroidManifest.xml  # مجوزهای دسترسی، سرویس و BootReceiver
            ├── java/com/netmetr/app/
            │   ├── MainActivity.kt               # ناوبری و رابط کاربری اصلی
            │   ├── NetMetrApp.kt                 # کانال نوتیفیکیشن
            │   ├── service/
            │   │   ├── NetworkSpeedService.kt    # سرویس پیش‌زمینه و نوتیفیکیشن دائمی سرعت و حجم
            │   │   └── BootReceiver.kt           # راه‌اندازی پس از ریبوت گوشی
            │   ├── data/
            │   │   ├── Models.kt
            │   │   └── UsageTracker.kt           # ثبت و ذخیره مصرف روزانه Wi-Fi و سیم‌کارت
            │   └── ui/
            │       ├── theme/                    # تم دارک نئونی متریال ۳
            │       ├── components/               # کارت‌های سرعت و مصرف روزانه
            │       └── screens/                  # صفحات داشبورد، تاریخچه و تنظیمات
            └── res/                              # آیکون‌ها و منابع گرافیکی
```

---

## 📄 مجوز (License)
این پروژه تحت پروانه [MIT](LICENSE) منتشر شده است.
