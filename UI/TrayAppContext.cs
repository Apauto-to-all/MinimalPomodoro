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

        var pauseItem = new ToolStripMenuItem() { Image = IconGenerator.GetPauseIcon(), Text = Localization.Get("暂停") };
        pauseItem.Click += (s, e) => _engine.TogglePause();
        menu.Items.Add(pauseItem);

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

        var exitItem = new ToolStripMenuItem() { Image = IconGenerator.GetExitIcon(), Text = Localization.Get("退出") };
        exitItem.Click += (s, e) => Exit();
        menu.Items.Add(exitItem);

        menu.Opening += (s, e) =>
        {
            // Update menu item states before showing
            if (_engine.CurrentState == PomodoroState.Paused)
            {
                pauseItem.Image?.Dispose();
                pauseItem.Image = IconGenerator.GetPlayIcon();
                pauseItem.Text = Localization.Get("继续");
            }
            else
            {
                pauseItem.Image?.Dispose();
                pauseItem.Image = IconGenerator.GetPauseIcon();
                pauseItem.Text = Localization.Get("暂停");
            }
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
        _engine.SaveSessionState();
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
