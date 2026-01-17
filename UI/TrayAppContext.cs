using System.Diagnostics;
using MinimalPomodoro.Models;
using MinimalPomodoro.Services;

namespace MinimalPomodoro.UI;

public class TrayAppContext : ApplicationContext
{
    private readonly NotifyIcon _notifyIcon;
    private readonly PomodoroEngine _engine;
    private readonly AppConfig _config;

    public TrayAppContext()
    {
        _config = ConfigManager.Load();

        // Handle initial language setup
        if (string.IsNullOrEmpty(_config.Language))
        {
            Localization.SetLanguage(null);
            _config.Language = Localization.GetCurrentLanguage();
            ConfigManager.Save(_config);
        }
        else
        {
            Localization.SetLanguage(_config.Language);
        }

        _engine = new PomodoroEngine(_config);

        // Sync startup shortcut in case the app was moved
        if (_config.AutoStart)
        {
            StartupManager.SetEnabled(true);
        }

        _notifyIcon = new NotifyIcon
        {
            Visible = true,
            ContextMenuStrip = CreateContextMenu()
        };

        _notifyIcon.MouseDoubleClick += OnIconDoubleClick;

        _engine.Tick += UpdateTray;
        _engine.Tick += () =>
        {
            // Early warning notifications based on user settings
            if (_config.EarlyWarningSecondsWork > 0 &&
                _engine.CurrentState == PomodoroState.Working &&
                _engine.RemainingSeconds == _config.EarlyWarningSecondsWork &&
                _engine.TotalSeconds > _config.EarlyWarningSecondsWork + 10)
            {
                _notifyIcon.ShowBalloonTip(3000,
                    Localization.Get("极简番茄钟"),
                    string.Format(Localization.Get("工作即将结束 (剩{0}秒)"), _config.EarlyWarningSecondsWork),
                    ToolTipIcon.Info);
            }
            else if (_config.EarlyWarningSecondsBreak > 0 &&
                    (_engine.CurrentState == PomodoroState.ShortBreak || _engine.CurrentState == PomodoroState.LongBreak) &&
                    _engine.RemainingSeconds == _config.EarlyWarningSecondsBreak &&
                    _engine.TotalSeconds > _config.EarlyWarningSecondsBreak + 10)
            {
                _notifyIcon.ShowBalloonTip(3000,
                    Localization.Get("极简番茄钟"),
                    string.Format(Localization.Get("休息即将结束 (剩{0}秒)"), _config.EarlyWarningSecondsBreak),
                    ToolTipIcon.Info);
            }

            // Auto-save session every minute to prevent loss on crash
            if (_engine.RemainingSeconds % 60 == 0)
            {
                _engine.SaveSessionState();
                ConfigManager.Save(_config);
            }
        };
        _engine.StateChanged += UpdateTray;
        _engine.TimerFinished += OnTimerFinished;

        UpdateTray();
    }

    private ContextMenuStrip CreateContextMenu()
    {
        var menu = new ContextMenuStrip();
        menu.ShowImageMargin = true;

        // Add custom control panel at the top
        var panel = new TrayControlPanel(_engine, _config);
        var host = new ToolStripControlHost(panel)
        {
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            AutoSize = false,
            Size = panel.Size
        };
        menu.Items.Add(host);
        menu.Items.Add(new ToolStripSeparator());

        var settingsItem = new ToolStripMenuItem() { Image = IconGenerator.GetSettingsIcon(), Text = Localization.Get("设置") };
        settingsItem.Click += (s, e) =>
        {
            using var form = new SettingsForm(_config);
            form.ShowDialog();
            if (form.SettingsChanged)
            {
                Application.Restart();
            }
        };
        menu.Items.Add(settingsItem);

        menu.Items.Add(new ToolStripSeparator());

        var resetItem = new ToolStripMenuItem() { Image = IconGenerator.GetResetIcon(), Text = Localization.Get("重置并重启") };
        resetItem.Click += (s, e) =>
        {
            StartupManager.SetEnabled(false);
            ConfigManager.Reset();
            Application.Restart();
        };
        menu.Items.Add(resetItem);

        var openConfigItem = new ToolStripMenuItem() { Image = IconGenerator.GetOpenConfigIcon(), Text = Localization.Get("打开配置文件夹") };
        openConfigItem.Click += (s, e) => { Process.Start("explorer.exe", ConfigManager.GetConfigPath()); };
        menu.Items.Add(openConfigItem);

        var autoStartItem = new ToolStripMenuItem() { Image = IconGenerator.GetAutoStartIcon(_config.AutoStart), Text = Localization.Get("开机自启") };
        autoStartItem.Click += (s, e) =>
        {
            _config.AutoStart = !_config.AutoStart;
            StartupManager.SetEnabled(_config.AutoStart);
            ConfigManager.Save(_config);
            autoStartItem.Image?.Dispose();
            autoStartItem.Image = IconGenerator.GetAutoStartIcon(_config.AutoStart);
        };
        menu.Items.Add(autoStartItem);

        // Language sub-menu
        var langMenuItem = new ToolStripMenuItem() { Image = IconGenerator.GetLanguageIcon(), Text = Localization.Get("语言") };

        var zhItem = new ToolStripMenuItem() { Text = "中文" };
        zhItem.Click += (s, e) => { UpdateLanguage("zh"); };

        var enItem = new ToolStripMenuItem() { Text = "English" };
        enItem.Click += (s, e) => { UpdateLanguage("en"); };

        langMenuItem.DropDownItems.Add(zhItem);
        langMenuItem.DropDownItems.Add(enItem);
        menu.Items.Add(langMenuItem);

        menu.Items.Add(new ToolStripSeparator());

        var version = Application.ProductVersion.Split('+')[0];
        var versionItem = new ToolStripMenuItem() { Text = $"v{version}", Enabled = false };
        menu.Items.Add(versionItem);

        var exitItem = new ToolStripMenuItem() { Image = IconGenerator.GetExitIcon(), Text = Localization.Get("退出") };
        exitItem.Click += (s, e) => Exit();
        menu.Items.Add(exitItem);

        menu.Opening += (s, e) =>
        {
            // Update menu item states before showing
            autoStartItem.Checked = _config.AutoStart;
            // update auto start icon to reflect state
            autoStartItem.Image?.Dispose();
            autoStartItem.Image = IconGenerator.GetAutoStartIcon(_config.AutoStart);

            // Update language checks
            zhItem.Checked = _config.Language == "zh";
            enItem.Checked = _config.Language == "en";
        };

        return menu;
    }

    private void UpdateLanguage(string lang)
    {
        if (_config.Language != lang)
        {
            _config.Language = lang;
            _engine.SaveSessionState();
            ConfigManager.Save(_config);
            Application.Restart();
        }
    }

    private void UpdateTray()
    {
        var oldIcon = _notifyIcon.Icon;
        _notifyIcon.Icon = IconGenerator.GenerateTomatoIcon(_engine.GetProgress(), _engine.CurrentState);
        _notifyIcon.Text = _engine.GetStatusText();

        // Clean up old icon to prevent memory leak
        oldIcon?.Dispose();
    }

    private void OnIconDoubleClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _engine.TogglePause();
        }
    }

    private void OnTimerFinished()
    {
        string message = _engine.CurrentState switch
        {
            PomodoroState.Working => Localization.Get("工作结束！该休息一下了。"),
            _ => Localization.Get("休息结束！准备开始工作了吗？")
        };

        _notifyIcon.ShowBalloonTip(3000, Localization.Get("极简番茄钟"), message, ToolTipIcon.Info);

        if (_config.SoundEnabled)
        {
            System.Media.SystemSounds.Beep.Play();
        }
    }

    private void Exit()
    {
        _engine.ClearSessionState();
        ConfigManager.Save(_config);
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        Application.Exit();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _notifyIcon?.Dispose();
        }
        base.Dispose(disposing);
    }
}
