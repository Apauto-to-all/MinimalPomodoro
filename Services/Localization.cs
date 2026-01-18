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
        },
        ["ja"] = new()
        {
            ["暂停"] = "一時停止",
            ["继续"] = "再開",
            ["重置并重启"] = "リセットして再起動",
            ["打开配置文件夹"] = "設定フォルダを開く",
            ["开机自启"] = "システム起動時に実行",
            ["退出"] = "終了",
            ["工作"] = "仕事",
            ["短休息"] = "短い休憩",
            ["长休息"] = "長い休憩",
            ["极简番茄钟"] = "ミニマル・ポモドーロ",
            ["首次使用建议"] = "初回ガイド",
            ["💡 双击图标：暂停/继续\n⚙️ 右键：打开控制面板与设置"] = "💡 アイコンをダブルクリック：一時停止/再開\n⚙️ 右クリック：コントロールパネルと設定を開く",
            ["项目主页 (GitHub)"] = "プロジェクトページ (GitHub)",
            ["工作结束！该休息一下了。"] = "仕事終了！休憩の時間です。",
            ["休息结束！准备开始工作了吗？"] = "休憩終了！仕事を始めますか？",
            ["语言"] = "言語",
            ["设置"] = "設定",
            ["工作时长 (分钟)"] = "仕事の時間 (分)",
            ["短休息时长 (分钟)"] = "短い休憩 (分)",
            ["长休息时长 (分钟)"] = "長い休憩 (分)",
            ["长休息间隔 (周期)"] = "長い休憩の間隔 (サイクル)",
            ["预先提醒-工作 (秒)"] = "仕事終了の事前通知 (秒)",
            ["预先提醒-休息 (秒)"] = "休憩終了の事前通知 (秒)",
            ["设置为0则禁用预警通知"] = "0に設定すると通知を無効にします",
            ["保存"] = "保存",
            ["应用已在运行"] = "アプリは既に実行中です。",
            ["工作即将结束 (剩{0}秒)"] = "仕事が間もなく終了します (残り{0}秒)",
            ["休息即将结束 (剩{0}秒)"] = "休憩が間もなく終了します (残り{0}秒)"
        },
        ["de"] = new()
        {
            ["暂停"] = "Pause",
            ["继续"] = "Fortsetzen",
            ["重置并重启"] = "Zurücksetzen & Neustart",
            ["打开配置文件夹"] = "Konfigurationsordner öffnen",
            ["开机自启"] = "Mit Windows starten",
            ["退出"] = "Beenden",
            ["工作"] = "Arbeit",
            ["短休息"] = "Kurze Pause",
            ["长休息"] = "Lange Pause",
            ["极简番茄钟"] = "Minimal Pomodoro",
            ["首次使用建议"] = "Willkommens-Guide",
            ["💡 双击图标：暂停/继续\n⚙️ 右键：打开控制面板与设置"] = "💡 Doppelklick auf Icon: Pause/Fortsetzen\n⚙️ Rechtsklick: Menü & Einstellungen öffnen",
            ["项目主页 (GitHub)"] = "Projekt-Homepage (GitHub)",
            ["工作结束！该休息一下了。"] = "Arbeitsphase beendet! Zeit für eine Pause.",
            ["休息结束！准备开始工作了吗？"] = "Pause beendet! Bereit für die Arbeit?",
            ["语言"] = "Sprache",
            ["设置"] = "Einstellungen",
            ["工作时长 (分钟)"] = "Arbeitsdauer (Min.)",
            ["短休息时长 (分钟)"] = "Kurze Pause (Min.)",
            ["长休息时长 (分钟)"] = "Lange Pause (Min.)",
            ["长休息间隔 (周期)"] = "Langes Pausenintervall",
            ["预先提醒-工作 (秒)"] = "Vorwarnung Arbeit (Sek.)",
            ["预先提醒-休息 (秒)"] = "Vorwarnung Pause (Sek.)",
            ["设置为0则禁用预警通知"] = "Auf 0 setzen, um Benachrichtigungen zu deaktivieren",
            ["保存"] = "Speichern",
            ["应用已在运行"] = "Anwendung läuft bereits.",
            ["工作即将结束 (剩{0}秒)"] = "Arbeitsphase endet bald ({0} Sek. übrig)",
            ["休息即将结束 (剩{0}秒)"] = "Pause endet bald ({0} Sek. übrig)"
        },
        ["es"] = new()
        {
            ["暂停"] = "Pausar",
            ["继续"] = "Reanudar",
            ["重置并重启"] = "Restablecer y Reiniciar",
            ["打开配置文件夹"] = "Abrir carpeta de configuración",
            ["开机自启"] = "Iniciar con Windows",
            ["退出"] = "Salir",
            ["工作"] = "Trabajo",
            ["短休息"] = "Descanso corto",
            ["长休息"] = "Descanso largo",
            ["极简番茄钟"] = "Minimal Pomodoro",
            ["首次使用建议"] = "Guía de bienvenida",
            ["💡 双击图标：暂停/继续\n⚙️ 右键：打开控制面板与设置"] = "💡 Doble clic: Pausar/Reanudar\n⚙️ Clic derecho: Panel y Ajustes",
            ["项目主页 (GitHub)"] = "Página del proyecto (GitHub)",
            ["工作结束！该休息一下了。"] = "¡Sesión terminada! Es hora de descansar.",
            ["休息结束！准备开始工作了吗？"] = "¡Descanso terminado! ¿Listo para trabajar?",
            ["语言"] = "Idioma",
            ["设置"] = "Ajustes",
            ["工作时长 (分钟)"] = "Duración de trabajo (min)",
            ["短休息时长 (分钟)"] = "Descanso corto (min)",
            ["长休息时长 (分钟)"] = "Descanso largo (min)",
            ["长休息间隔 (周期)"] = "Intervalo de descanso largo",
            ["预先提醒-工作 (秒)"] = "Aviso previo trabajo (seg)",
            ["预先提醒-休息 (秒)"] = "Aviso previo descanso (seg)",
            ["设置为0则禁用预警通知"] = "Establecer a 0 para desactivar avisos",
            ["保存"] = "Guardar",
            ["应用已在运行"] = "La aplicación ya está en ejecución.",
            ["工作即将结束 (剩{0}秒)"] = "El trabajo terminará pronto ({0}s restantes)",
            ["休息即将结束 (剩{0}秒)"] = "El descanso terminará pronto ({0}s restantes)"
        },
        ["fr"] = new()
        {
            ["暂停"] = "Pause",
            ["继续"] = "Reprendre",
            ["重置并重启"] = "Réinitialiser & Redémarrer",
            ["打开配置文件夹"] = "Ouvrir le dossier de config",
            ["开机自启"] = "Lancer au démarrage",
            ["退出"] = "Quitter",
            ["工作"] = "Travail",
            ["短休息"] = "Pause courte",
            ["长休息"] = "Pause longue",
            ["极简番茄钟"] = "Minimal Pomodoro",
            ["首次使用建议"] = "Guide de bienvenue",
            ["💡 双击图标：暂停/继续\n⚙️ 右键：打开控制面板与设置"] = "💡 Double-clic : Pause/Reprendre\n⚙️ Clic droit : Panneau & Réglages",
            ["项目主页 (GitHub)"] = "Page du projet (GitHub)",
            ["工作结束！该休息一下了。"] = "Travail terminé ! C'est l'heure de la pause.",
            ["休息结束！准备开始工作了吗？"] = "Pause terminée ! Prêt à travailler ?",
            ["语言"] = "Langue",
            ["设置"] = "Réglages",
            ["工作时长 (分钟)"] = "Durée du travail (min)",
            ["短休息时长 (分钟)"] = "Pause courte (min)",
            ["长休息时长 (分钟)"] = "Pause longue (min)",
            ["长休息间隔 (周期)"] = "Intervalle pause longue",
            ["预先提醒-工作 (秒)"] = "Pré-alerte travail (sec)",
            ["预先提醒-休息 (秒)"] = "Pré-alerte pause (sec)",
            ["设置为0则禁用预警通知"] = "Régler à 0 pour désactiver l'alerte",
            ["保存"] = "Enregistrer",
            ["应用已在运行"] = "L'application est déjà en cours d'exécution.",
            ["工作即将结束 (剩{0}秒)"] = "Travail bientôt terminé ({0}s restantes)",
            ["休息即将结束 (剩{0}秒)"] = "Pause bientôt terminée ({0}s restantes)"
        },
        ["ko"] = new()
        {
            ["暂停"] = "일시 중지",
            ["继续"] = "재개",
            ["重置并重启"] = "초기화 및 재시작",
            ["打开配置文件夹"] = "설정 폴더 열기",
            ["开机自启"] = "시작 시 실행",
            ["退出"] = "종료",
            ["工作"] = "작업",
            ["短休息"] = "짧은 휴식",
            ["长休息"] = "긴 휴식",
            ["极简番茄钟"] = "미니멀 뽀모도로",
            ["首次使用建议"] = "환영합니다! 사용 가이드",
            ["💡 双击图标：暂停/继续\n⚙️ 右键：打开控制面板与设置"] = "💡 아이콘 더블 클릭: 일시 중지/재개\n⚙️ 우클릭: 제어판 및 설정 열기",
            ["项目主页 (GitHub)"] = "프로젝트 페이지 (GitHub)",
            ["工作结束！该休息一下了。"] = "작업 종료! 휴식 시간입니다.",
            ["休息结束！准备开始工作了吗？"] = "휴식 종료! 작업을 시작할까요?",
            ["语言"] = "언어",
            ["设置"] = "설정",
            ["工作时长 (分钟)"] = "작업 시간 (분)",
            ["短休息时长 (分钟)"] = "짧은 휴식 (분)",
            ["长休息时长 (分钟)"] = "긴 휴식 (분)",
            ["长休息间隔 (周期)"] = "긴 휴식 간격 (사이클)",
            ["预先提醒-工作 (秒)"] = "작업 종료 전 알림 (초)",
            ["预先提醒-休息 (秒)"] = "휴식 종료 전 알림 (초)",
            ["设置为0则禁用预警通知"] = "알림을 끄려면 0으로 설정하세요",
            ["保存"] = "저장",
            ["应用已在运行"] = "앱이 이미 실행 중입니다.",
            ["工作即将结束 (剩{0}秒)"] = "작업이 곧 종료됩니다 ({0}초 남음)",
            ["休息即将结束 (剩{0}秒)"] = "휴식이 곧 종료됩니다 ({0}초 남음)"
        }
    };

    public static void SetLanguage(string? language)
    {
        if (string.IsNullOrEmpty(language))
        {
            // Auto detect system language
            var sysLang = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            _currentLanguage = Translations.ContainsKey(sysLang) ? sysLang : (sysLang == "zh" ? "zh" : "en");
        }
        else
        {
            _currentLanguage = language;
        }
    }

    public static string GetCurrentLanguage() => _currentLanguage;

    public static string Get(string key)
    {
        if (_currentLanguage != "zh" && Translations.ContainsKey(_currentLanguage))
        {
            if (Translations[_currentLanguage].TryGetValue(key, out var translation))
            {
                return translation;
            }
        }
        return key;
    }
}
