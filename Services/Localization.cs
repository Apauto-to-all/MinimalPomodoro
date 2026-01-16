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
            ["工作结束！该休息一下了。"] = "Work session finished! Time for a break.",
            ["休息结束！准备开始工作了吗？"] = "Break finished! Ready to work?",
            ["语言"] = "Language"
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
