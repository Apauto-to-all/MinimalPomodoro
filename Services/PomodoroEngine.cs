using MinimalPomodoro.Models;

namespace MinimalPomodoro.Services;

public class PomodoroEngine
{
    private readonly AppConfig _config;
    private readonly System.Windows.Forms.Timer _timer;

    public PomodoroState CurrentState { get; private set; } = PomodoroState.Paused;
    public PomodoroState PreviousState { get; private set; } = PomodoroState.Working;
    public int CurrentCycle { get; private set; } = 1;
    public int RemainingSeconds { get; private set; }
    public int TotalSeconds { get; private set; }

    public event Action? StateChanged;
    public event Action? Tick;
    public event Action? TimerFinished;

    public PomodoroEngine(AppConfig config)
    {
        _config = config;
        _timer = new System.Windows.Forms.Timer { Interval = 1000 };
        _timer.Tick += OnTick;

        if (config.LastState.HasValue)
        {
            CurrentState = config.LastState.Value;
            PreviousState = config.LastPreviousState ?? PomodoroState.Working;
            CurrentCycle = config.LastCycle;
            RemainingSeconds = config.LastRemainingSeconds;

            // Recalculate TotalSeconds for progress bar
            var baseState = (CurrentState == PomodoroState.Paused) ? PreviousState : CurrentState;
            TotalSeconds = baseState switch
            {
                PomodoroState.Working => _config.WorkDurationMinutes * 60,
                PomodoroState.ShortBreak => _config.ShortBreakDurationMinutes * 60,
                PomodoroState.LongBreak => _config.LongBreakDurationMinutes * 60,
                _ => _config.WorkDurationMinutes * 60
            };

            // Always ensure we are in Paused state on startup
            if (CurrentState != PomodoroState.Paused)
            {
                PreviousState = CurrentState;
                CurrentState = PomodoroState.Paused;
            }
        }
        else
        {
            // Initial startup or no saved session
            CurrentState = PomodoroState.Paused;
            PreviousState = PomodoroState.Working;
            CurrentCycle = 1;
            TotalSeconds = _config.WorkDurationMinutes * 60;
            RemainingSeconds = TotalSeconds;
        }
    }

    public void SaveSessionState()
    {
        _config.LastState = CurrentState;
        _config.LastPreviousState = PreviousState;
        _config.LastRemainingSeconds = RemainingSeconds;
        _config.LastCycle = CurrentCycle;
    }

    public void ClearSessionState()
    {
        _config.LastState = null;
        _config.LastPreviousState = null;
        _config.LastRemainingSeconds = 0;
        _config.LastCycle = 1;
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (RemainingSeconds > 0)
        {
            RemainingSeconds--;
            Tick?.Invoke();
        }
        else
        {
            _timer.Stop();
            HandleFinish();
        }
    }

    private void HandleFinish()
    {
        TimerFinished?.Invoke();

        if (CurrentState == PomodoroState.Working)
        {
            if (CurrentCycle >= _config.LongBreakInterval)
            {
                StartLongBreak();
            }
            else
            {
                StartShortBreak();
            }
        }
        else
        {
            // After break, move to next cycle and start work
            CurrentCycle = (CurrentState == PomodoroState.LongBreak) ? 1 : CurrentCycle + 1;
            ResetToWork();
            Start();
        }
    }

    public void StartWork()
    {
        CurrentState = PomodoroState.Working;
        TotalSeconds = _config.WorkDurationMinutes * 60;
        RemainingSeconds = TotalSeconds;
        _timer.Start();
        StateChanged?.Invoke();
    }

    public void StartShortBreak()
    {
        CurrentState = PomodoroState.ShortBreak;
        TotalSeconds = _config.ShortBreakDurationMinutes * 60;
        RemainingSeconds = TotalSeconds;
        _timer.Start();
        StateChanged?.Invoke();
    }

    public void StartLongBreak()
    {
        CurrentState = PomodoroState.LongBreak;
        TotalSeconds = _config.LongBreakDurationMinutes * 60;
        RemainingSeconds = TotalSeconds;
        _timer.Start();
        StateChanged?.Invoke();
    }

    public void TogglePause()
    {
        if (CurrentState == PomodoroState.Paused)
        {
            CurrentState = PreviousState;
            _timer.Start();
        }
        else
        {
            PreviousState = CurrentState;
            CurrentState = PomodoroState.Paused;
            _timer.Stop();
        }
        StateChanged?.Invoke();
    }

    public void NextStage()
    {
        _timer.Stop();
        if (CurrentState == PomodoroState.Working || (CurrentState == PomodoroState.Paused && PreviousState == PomodoroState.Working))
        {
            if (CurrentCycle >= _config.LongBreakInterval)
            {
                StartLongBreak();
            }
            else
            {
                StartShortBreak();
            }
        }
        else if (CurrentState == PomodoroState.ShortBreak || (CurrentState == PomodoroState.Paused && PreviousState == PomodoroState.ShortBreak))
        {
            CurrentCycle++;
            StartWork();
        }
        else if (CurrentState == PomodoroState.LongBreak || (CurrentState == PomodoroState.Paused && PreviousState == PomodoroState.LongBreak))
        {
            CurrentCycle = 1;
            StartWork();
        }
    }

    public void PreviousStage()
    {
        _timer.Stop();
        if (CurrentState == PomodoroState.Working || (CurrentState == PomodoroState.Paused && PreviousState == PomodoroState.Working))
        {
            if (CurrentCycle > 1)
            {
                CurrentCycle--;
                StartShortBreak();
            }
            else
            {
                CurrentCycle = _config.LongBreakInterval;
                StartLongBreak();
            }
        }
        else if (CurrentState == PomodoroState.ShortBreak || (CurrentState == PomodoroState.Paused && PreviousState == PomodoroState.ShortBreak))
        {
            StartWork();
        }
        else if (CurrentState == PomodoroState.LongBreak || (CurrentState == PomodoroState.Paused && PreviousState == PomodoroState.LongBreak))
        {
            StartWork();
        }
    }

    public void Start()
    {
        if (CurrentState == PomodoroState.Paused)
        {
            TogglePause();
        }
        else
        {
            _timer.Start();
        }
    }

    public void ResetToWork()
    {
        CurrentState = PomodoroState.Working;
        TotalSeconds = _config.WorkDurationMinutes * 60;
        RemainingSeconds = TotalSeconds;
        _timer.Stop();
        StateChanged?.Invoke();
    }

    public float GetProgress() => TotalSeconds == 0 ? 0 : (float)RemainingSeconds / TotalSeconds;

    public string GetStatusText()
    {
        string stateKey = CurrentState switch
        {
            PomodoroState.Working => "工作",
            PomodoroState.ShortBreak => "短休息",
            PomodoroState.LongBreak => "长休息",
            _ => "暂停"
        };
        string stateText = Localization.Get(stateKey);
        TimeSpan t = TimeSpan.FromSeconds(RemainingSeconds);
        return $"{stateText} ({CurrentCycle}/{_config.LongBreakInterval}) - {t.Minutes:D2}:{t.Seconds:D2}";
    }
}
