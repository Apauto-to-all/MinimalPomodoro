using System.Collections.Generic;

namespace MinimalPomodoro.Services;

public static class Localization
{
    private static string _currentLanguage = "zh";

    private static readonly Dictionary<string, Dictionary<string, string>> Translations = new()
    {
        ["en"] = new()
        {
            ["暂停"] = "Pause",
            ["继续"] = "Resume",
            ["重置并重启"] = "Reset & Restart",
            ["打开配置文件夹"] = "Open Config Folder",
            ["开机自启"] = "Launch at Startup",
            ["退出"] = "Exit",
            ["工作"] = "Work",
            ["短休息"] = "Short Break",
            ["长休息"] = "Long Break",
            ["极简番茄钟"] = "Minimal Pomodoro",
            ["首次使用建议"] = "Welcome! Quick Start Guide",
            ["💡 双击图标：暂停/继续\n⚙️ 右键：打开控制面板与设置"] = "💡 Double-click icon: Pause/Resume\n⚙️ Right-click: Open panel & settings",
            ["项目主页 (GitHub)"] = "Project Homepage (GitHub)",
            ["工作结束！该休息一下了。"] = "Work session finished! Time for a break.",
            ["休息结束！准备开始工作了吗？"] = "Break finished! Ready to work?",
            ["语言"] = "Language",
            ["设置"] = "Settings",
            ["工作时长 (分钟)"] = "Work Duration (min)",
            ["短休息时长 (分钟)"] = "Short Break (min)",
            ["长休息时长 (分钟)"] = "Long Break (min)",
            ["长休息间隔 (周期)"] = "Long Break Interval",
            ["预先提醒-工作 (秒)"] = "Pre-warn Work (sec)",
            ["预先提醒-休息 (秒)"] = "Pre-warn Break (sec)",
            ["设置为0则禁用预警通知"] = "Set to 0 to disable pre-warning notifications",
            ["保存"] = "Save",
            ["应用已在运行"] = "Application is already running.",
            ["工作即将结束 (剩{0}秒)"] = "Work session ending soon ({0}s left)",
            ["休息即将结束 (剩{0}秒)"] = "Break session ending soon ({0}s left)"
        }
    };

    public static void SetLanguage(string? language)
    {
        if (string.IsNullOrEmpty(language))
        {
            // Auto detect system language
            var sysLang = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            _currentLanguage = (sysLang == "zh") ? "zh" : "en";
        }
        else
        {
            _currentLanguage = language;
        }
    }

    public static string GetCurrentLanguage() => _currentLanguage;

    public static string Get(string key)
    {
        if (_currentLanguage == "en" && Translations["en"].TryGetValue(key, out var translation))
        {
            return translation;
        }
        return key;
    }
}
