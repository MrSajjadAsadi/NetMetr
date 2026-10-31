# ProGuard configuration for NetMetr
-keepattributes *Annotation*
-keepclassmembers class * {
    @org.jetbrains.annotations.* <fields>;
    @org.jetbrains.annotations.* <methods>;
}
-keep class com.google.gson.** { *; }
-keep class com.netmetr.app.data.** { *; }
