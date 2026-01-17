using System.Drawing;
using System.Drawing.Drawing2D;
using MinimalPomodoro.Models;
using MinimalPomodoro.Services;

namespace MinimalPomodoro.UI;

public class TrayControlPanel : UserControl
{
    private readonly PomodoroEngine _engine;
    private readonly AppConfig _config;

    private readonly Label _lblStatus;
    private readonly Label _lblTime;
    private readonly Button _btnPrev;
    private readonly Button _btnToggle;
    private readonly Button _btnNext;
    private readonly Panel _progressContainer;

    public TrayControlPanel(PomodoroEngine engine, AppConfig config)
    {
        _engine = engine;
        _config = config;

        this.Size = new Size(240, 100);
        this.BackColor = Color.White;
        this.Padding = new Padding(12, 8, 12, 8);

        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25)); // 1. Text
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 16)); // 2. Progress Bar
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // 3. Buttons

        // 1. Status and Time
        var textLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0)
        };
        textLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        textLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

        _lblStatus = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Microsoft YaHei", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(45, 45, 45)
        };
        _lblTime = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
            Font = new Font("Consolas", 10, FontStyle.Regular),
            ForeColor = Color.FromArgb(100, 100, 100)
        };
        textLayout.Controls.Add(_lblStatus, 0, 0);
        textLayout.Controls.Add(_lblTime, 1, 0);

        // 2. Progress Bar Container
        _progressContainer = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 4, 0, 4)
        };
        _progressContainer.Paint += PaintProgress;

        // 3. Control Buttons
        var buttonLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            // Center buttons
            Padding = new Padding(38, 0, 0, 0),
            Margin = new Padding(0)
        };

        _btnPrev = CreateControlButton(IconGenerator.GetControlPrevIcon());
        _btnToggle = CreateControlButton(IconGenerator.GetControlPlayIcon());
        _btnToggle.Tag = "play"; // Initial tag
        _btnNext = CreateControlButton(IconGenerator.GetControlNextIcon());

        _btnPrev.Click += (s, e) => _engine.PreviousStage();
        _btnToggle.Click += (s, e) => _engine.TogglePause();
        _btnNext.Click += (s, e) => _engine.NextStage();

        buttonLayout.Controls.Add(_btnPrev);
        buttonLayout.Controls.Add(_btnToggle);
        buttonLayout.Controls.Add(_btnNext);

        mainLayout.Controls.Add(textLayout, 0, 0);
        mainLayout.Controls.Add(_progressContainer, 0, 1);
        mainLayout.Controls.Add(buttonLayout, 0, 2);

        this.Controls.Add(mainLayout);

        _engine.Tick += UpdateUI;
        _engine.StateChanged += UpdateUI;

        UpdateUI();
    }

    private Button CreateControlButton(Image image)
    {
        var btn = new Button
        {
            Image = image,
            Size = new Size(42, 38),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(2, 0, 2, 0)
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(242, 242, 242);
        btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(230, 230, 230);
        return btn;
    }

    private void UpdateUI()
    {
        if (this.IsDisposed) return;
        if (this.InvokeRequired)
        {
            _ = this.BeginInvoke(new Action(UpdateUI));
            return;
        }

        var state = _engine.CurrentState == PomodoroState.Paused ? _engine.PreviousState : _engine.CurrentState;
        string stateKey = state switch
        {
            PomodoroState.Working => "工作",
            PomodoroState.ShortBreak => "短休息",
            PomodoroState.LongBreak => "长休息",
            _ => "工作中"
        };

        _lblStatus.Text = $"{Localization.Get(stateKey)} {_engine.CurrentCycle}/{_config.LongBreakInterval}";

        int elapsedSeconds = _engine.TotalSeconds - _engine.RemainingSeconds;
        TimeSpan elapsed = TimeSpan.FromSeconds(elapsedSeconds);
        _lblTime.Text = $"{elapsed:mm\\:ss}";

        // Update toggle button icon only when state changes
        bool isPaused = _engine.CurrentState == PomodoroState.Paused;
        bool currentlyIconIsPlay = _btnToggle.Tag as string == "play";

        if (isPaused && !currentlyIconIsPlay)
        {
            var oldImage = _btnToggle.Image;
            _btnToggle.Image = IconGenerator.GetControlPlayIcon();
            _btnToggle.Tag = "play";
            oldImage?.Dispose();
        }
        else if (!isPaused && currentlyIconIsPlay)
        {
            var oldImage = _btnToggle.Image;
            _btnToggle.Image = IconGenerator.GetControlPauseIcon();
            _btnToggle.Tag = "pause";
            oldImage?.Dispose();
        }

        _progressContainer.Invalidate();
    }

    private void PaintProgress(object? sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Use full height of the container minus a small padding for the rounded look
        int h = _progressContainer.Height - 2;
        int y = 1;

        // Background track (rounded)
        using (var path = CreateRoundedRect(new Rectangle(0, y, _progressContainer.Width, h), h / 2))
        using (var brush = new SolidBrush(Color.FromArgb(245, 245, 245)))
        {
            e.Graphics.FillPath(brush, path);
        }

        // Active track
        float progress = _engine.TotalSeconds == 0 ? 0 : (float)(_engine.TotalSeconds - _engine.RemainingSeconds) / _engine.TotalSeconds;
        int fillWidth = (int)(_progressContainer.Width * progress);

        if (fillWidth > h) // Only draw if enough space for rounding
        {
            var state = _engine.CurrentState == PomodoroState.Paused ? _engine.PreviousState : _engine.CurrentState;
            Color barColor = state switch
            {
                PomodoroState.Working => Color.FromArgb(231, 76, 60),
                PomodoroState.ShortBreak => Color.FromArgb(39, 174, 96),
                PomodoroState.LongBreak => Color.FromArgb(41, 128, 185),
                _ => Color.Gray
            };

            if (_engine.CurrentState == PomodoroState.Paused)
            {
                barColor = Color.FromArgb(160, barColor); // More dimming
            }

            using (var path = CreateRoundedRect(new Rectangle(0, y, fillWidth, h), h / 2))
            using (var brush = new SolidBrush(barColor))
            {
                e.Graphics.FillPath(brush, path);
            }
        }
    }

    private GraphicsPath CreateRoundedRect(Rectangle rect, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        int d = radius * 2;
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}
