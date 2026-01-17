using System;
using System.Drawing;
using System.Windows.Forms;
using MinimalPomodoro.Models;
using MinimalPomodoro.Services;

namespace MinimalPomodoro.UI;

public class SettingsForm : Form
{
    private readonly AppConfig _config;
    private bool _isLoaded = false;
    private readonly ToolTip _toolTip;
    public bool SettingsChanged { get; private set; } = false;

    public SettingsForm(AppConfig config)
    {
        _config = config;
        _toolTip = new ToolTip();

        this.Text = $"{Localization.Get("极简番茄钟")} - {Localization.Get("设置")}";
        this.Size = new Size(600, 420);
        this.MinimumSize = new Size(580, 400);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.TopMost = true;
        this.BackColor = Color.White;
        this.Padding = new Padding(10);

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 7,
            Padding = new Padding(10)
        };

        // Column widths: Label(35%), TrackBar(45%), Numeric(20%)
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35f));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45f));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));

        // Row heights: Fixed height for each setting row
        for (int i = 0; i < 6; i++)
        {
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50f));
        }

        AddSettingRow(table, 0, Localization.Get("工作时长 (分钟)"), config.WorkDurationMinutes, 1, 120, (v) => _config.WorkDurationMinutes = v);
        AddSettingRow(table, 1, Localization.Get("短休息时长 (分钟)"), config.ShortBreakDurationMinutes, 1, 60, (v) => _config.ShortBreakDurationMinutes = v);
        AddSettingRow(table, 2, Localization.Get("长休息时长 (分钟)"), config.LongBreakDurationMinutes, 1, 60, (v) => _config.LongBreakDurationMinutes = v);
        AddSettingRow(table, 3, Localization.Get("长休息间隔 (周期)"), config.LongBreakInterval, 1, 10, (v) => _config.LongBreakInterval = v);

        AddSettingRow(table, 4, Localization.Get("预先提醒-工作 (秒)"), config.EarlyWarningSecondsWork, 0, 300, (v) => _config.EarlyWarningSecondsWork = v, Localization.Get("设置为0则禁用预警通知"));
        AddSettingRow(table, 5, Localization.Get("预先提醒-休息 (秒)"), config.EarlyWarningSecondsBreak, 0, 120, (v) => _config.EarlyWarningSecondsBreak = v, Localization.Get("设置为0则禁用预警通知"));

        this.Controls.Add(table);
        _isLoaded = true;
    }

    private void AddSettingRow(TableLayoutPanel table, int row, string labelText, int currentValue, int min, int max, Action<int> updateAction, string? toolTipText = null)
    {
        var label = new Label
        {
            Text = labelText,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 9f)
        };

        if (!string.IsNullOrEmpty(toolTipText))
        {
            _toolTip.SetToolTip(label, toolTipText);
        }

        var trackBar = new TrackBar
        {
            Minimum = min,
            Maximum = max,
            Value = Math.Clamp(currentValue, min, max),
            Dock = DockStyle.Fill,
            TickStyle = TickStyle.None,
            Height = 30
        };

        if (!string.IsNullOrEmpty(toolTipText))
        {
            _toolTip.SetToolTip(trackBar, toolTipText);
        }

        var numeric = new NumericUpDown
        {
            Minimum = min,
            Maximum = max,
            Value = Math.Clamp(currentValue, min, max),
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9f),
            Margin = new Padding(5, 5, 0, 5)
        };

        if (!string.IsNullOrEmpty(toolTipText))
        {
            _toolTip.SetToolTip(numeric, toolTipText);
        }

        // Sync logic
        trackBar.ValueChanged += (s, e) =>
        {
            if (numeric.Value != trackBar.Value)
            {
                numeric.Value = trackBar.Value;
            }
        };

        numeric.ValueChanged += (s, e) =>
        {
            if (trackBar.Value != (int)numeric.Value)
            {
                trackBar.Value = (int)numeric.Value;
            }

            if (_isLoaded)
            {
                updateAction((int)numeric.Value);
                ConfigManager.Save(_config);
                SettingsChanged = true;
            }
        };

        table.Controls.Add(label, 0, row);
        table.Controls.Add(trackBar, 1, row);
        table.Controls.Add(numeric, 2, row);
    }
}
