using System.Timers;
using System.Windows.Input;
using TimeFlow.Domain.Entities;
using Microsoft.Maui.Dispatching;
using Timer = System.Timers.Timer;

namespace TimeFlow.Presentation.ViewModels
{
    public class PomodoroViewModel : BaseViewModel
    {
        private readonly IDispatcher _dispatcher;
        private Timer _timer;
        private int _remainingTime; // in seconds
        private bool _isRunning;

        public int RemainingTime
        {
            get => _remainingTime;
            set
            {
                if (_remainingTime != value)
                {
                    _remainingTime = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TimeDisplay));
                }
            }
        }

        public string TimeDisplay => TimeSpan.FromSeconds(RemainingTime).ToString(@"mm\:ss");

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanStart));
                    OnPropertyChanged(nameof(CanPause));
                    OnPropertyChanged(nameof(CanReset));
                }
            }
        }

        public ICommand StartCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ResetCommand { get; }

        // Default durations (in minutes)
        private int _workDuration = 25;
        private int _shortBreakDuration = 5;
        private int _longBreakDuration = 15;

        private PomodoroSessionType _currentSessionType;
        private int _sessionCount = 0; 

        public PomodoroViewModel()
        {
            _dispatcher = Dispatcher.GetForCurrentThread();
            RemainingTime = _workDuration * 60;
            _currentSessionType = PomodoroSessionType.Work;

            _timer = new Timer(1000);
            _timer.Elapsed += OnTimerElapsed;

            StartCommand = new Command(StartTimer, () => CanStart);
            PauseCommand = new Command(PauseTimer, () => CanPause);
            ResetCommand = new Command(ResetTimer, () => CanReset);
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            RemainingTime--;

            if (RemainingTime <= 0)
            {
                _timer.Stop();
                IsRunning = false;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    OnSessionCompleted();
                });
            }
        }

        private void OnSessionCompleted()
        {
            if (_currentSessionType == PomodoroSessionType.Work)
            {
                _sessionCount++;
                if (_sessionCount % 4 == 0)
                {
                    // Длинный отдых после 4х сессий
                    _currentSessionType = PomodoroSessionType.LongBreak;
                    RemainingTime = _longBreakDuration * 60;
                }
                else
                {
                    _currentSessionType = PomodoroSessionType.ShortBreak;
                    RemainingTime = _shortBreakDuration * 60;
                }
            }
            else
            {
                _currentSessionType = PomodoroSessionType.Work;
                RemainingTime = _workDuration * 60;
            }
        }

        public void StartTimer()
        {
            if (!IsRunning)
            {
                _timer.Start();
                IsRunning = true;
            }
        }

        public void PauseTimer()
        {
            if (IsRunning)
            {
                _timer.Stop();
                IsRunning = false;
            }
        }

        public void ResetTimer()
        {
            _timer.Stop();
            IsRunning = false;
            _sessionCount = 0;
            _currentSessionType = PomodoroSessionType.Work;
            RemainingTime = _workDuration * 60;
        }

        public bool CanStart => !IsRunning;
        public bool CanPause => IsRunning;
        public bool CanReset => true;
    }
}
