namespace MinimalPomodoro.Models;

public class AppConfig
{
    public int WorkDurationMinutes { get; set; } = 25;
    public int ShortBreakDurationMinutes { get; set; } = 5;
    public int LongBreakDurationMinutes { get; set; } = 15;
    public int LongBreakInterval { get; set; } = 4;
    public bool SoundEnabled { get; set; } = true;
    public bool AutoStart { get; set; } = false;
    public string? Language { get; set; }
    public int EarlyWarningSecondsWork { get; set; } = 60;
    public int EarlyWarningSecondsBreak { get; set; } = 30;
    public bool FirstRun { get; set; } = true;

    // Session persistence
    public PomodoroState? LastState { get; set; }
    public PomodoroState? LastPreviousState { get; set; }
    public int LastRemainingSeconds { get; set; }
    public int LastCycle { get; set; }
}
