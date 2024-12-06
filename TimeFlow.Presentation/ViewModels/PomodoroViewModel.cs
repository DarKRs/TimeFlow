using Plugin.Maui.Audio;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Presentation.ViewModels
{
    public class PomodoroViewModel : BaseViewModel
    {
        private readonly IDispatcher _dispatcher;
        private readonly ITaskService _taskService;
        private readonly IPomodoroSessionRepository _pomodoroSessionRepository;
        private readonly IAudioManager _audioManager;
        private readonly INotifyService _notificationService;
        private IAudioPlayer _audioPlayer;

        private IDispatcherTimer _timer;
        private int _remainingTime; // in seconds
        private bool _isRunning;
        private int _totalTimeForCurrentSession;

        private PomodoroSessionType _currentSessionType;
        private int _sessionCount = 0;

        private ObservableCollection<TaskItem> _todayTasks;
        public ObservableCollection<TaskItem> TodayTasks
        {
            get => _todayTasks;
            set => SetProperty(ref _todayTasks, value);
        }

        private int _workDuration;
        public int WorkDuration
        {
            get => _workDuration;
            set
            {
                if (SetProperty(ref _workDuration, value))
                {
                    OnPropertyChanged(nameof(DisplayWorkDuration));
                    if (_currentSessionType == PomodoroSessionType.Work && !IsRunning)
                    {
                        RemainingTime = WorkDuration * 60;
                    }
                }
            }
        }

        private int _shortBreakDuration;
        public int ShortBreakDuration
        {
            get => _shortBreakDuration;
            set
            {
                if (SetProperty(ref _shortBreakDuration, value))
                {
                    OnPropertyChanged(nameof(DisplayShortBreakDuration));
                    if (_currentSessionType == PomodoroSessionType.ShortBreak && !IsRunning)
                    {
                        RemainingTime = ShortBreakDuration * 60;
                    }
                }
            }
        }

        private int _longBreakDuration;
        public int LongBreakDuration
        {
            get => _longBreakDuration;
            set
            {
                if (SetProperty(ref _longBreakDuration, value))
                {
                    OnPropertyChanged(nameof(DisplayLongBreakDuration));
                    if (_currentSessionType == PomodoroSessionType.LongBreak && !IsRunning)
                    {
                        RemainingTime = LongBreakDuration * 60;
                    }
                }
            }
        }

        private TaskItem _selectedTask;
        public TaskItem SelectedTask
        {
            get => _selectedTask;
            set => SetProperty(ref _selectedTask, value);
        }

        public int RemainingTime
        {
            get => _remainingTime;
            set
            {
                if (_remainingTime != value)
                {
                    _remainingTime = value < 0 ? 0 : value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TimeDisplay));
                    OnPropertyChanged(nameof(Progress));
                }
            }
        }

        private bool _autoStartNextSession = true;
        public bool AutoStartNextSession
        {
            get => _autoStartNextSession;
            set => SetProperty(ref _autoStartNextSession, value);
        }

        public double Progress => (double)(_totalTimeForCurrentSession - RemainingTime) / _totalTimeForCurrentSession;

        public string TimeDisplay => TimeSpan.FromSeconds(RemainingTime).ToString(RemainingTime >= 3600 ? @"hh\:mm\:ss" : @"mm\:ss");

        public string DisplayWorkDuration => $"{WorkDuration} мин";
        public string DisplayShortBreakDuration => $"{ShortBreakDuration} мин";
        public string DisplayLongBreakDuration => $"{LongBreakDuration} мин";

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (SetProperty(ref _isRunning, value))
                {
                    UpdateCommandStates();
                }
            }
        }

        private bool _isPanelVisible;
        public bool IsPanelVisible
        {
            get => _isPanelVisible;
            set => SetProperty(ref _isPanelVisible, value);
        }

        private bool _isTasksTabVisible = true;
        public bool IsTasksTabVisible
        {
            get => _isTasksTabVisible;
            set => SetProperty(ref _isTasksTabVisible, value);
        }

        private bool _isSettingsTabVisible = false;
        public bool IsSettingsTabVisible
        {
            get => _isSettingsTabVisible;
            set => SetProperty(ref _isSettingsTabVisible, value);
        }

        public ICommand StartCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand TogglePanelCommand { get; }
        public ICommand ShowTasksTabCommand { get; }
        public ICommand ShowSettingsTabCommand { get; }

        public PomodoroViewModel(ITaskService taskService, IPomodoroSessionRepository pomodoroSessionRepository, IAudioManager audioManager, INotifyService notificationService)
        {
            _pomodoroSessionRepository = pomodoroSessionRepository;
            _taskService = taskService;
            _audioManager = audioManager;
            _notificationService = notificationService;

#if DEBUG
            _workDuration = 1; // 1 минута работы
            _shortBreakDuration = 1; // 1 минута короткого перерыва
            _longBreakDuration = 2; // 2 минуты длинного перерыва
#else
            _workDuration = 25;
            _shortBreakDuration = 5;
            _longBreakDuration = 15;
#endif

            _dispatcher = Dispatcher.GetForCurrentThread();

            RemainingTime = _workDuration * 60;
            _currentSessionType = PomodoroSessionType.Work;

            _timer = _dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;

            StartCommand = new Command(StartTimer, () => CanStart);
            PauseCommand = new Command(PauseTimer, () => CanPause);
            ResetCommand = new Command(ResetTimer, () => CanReset);
            TogglePanelCommand = new Command(() => IsPanelVisible = !IsPanelVisible);

            ShowTasksTabCommand = new Command(() =>
            {
                IsTasksTabVisible = true;
                IsSettingsTabVisible = false;
            });

            ShowSettingsTabCommand = new Command(() =>
            {
                IsTasksTabVisible = false;
                IsSettingsTabVisible = true;
            });

            TodayTasks = new ObservableCollection<TaskItem>();
            LoadTodayTasks();
            LoadAudio();
        }

        public async void LoadTodayTasks()
        {
            var todayTasks = await _taskService.GetTasksByDateAsync(DateTime.Today);
            var sortedTasks = todayTasks.OrderBy(task => task.Category).ToList();

            TodayTasks = new ObservableCollection<TaskItem>(sortedTasks);
        }

        public async Task UpdateTaskCompletionStatus(TaskItem task)
        {
            if (task != null)
            {
                await _taskService.UpdateTaskAsync(task);
            }
        }

        private async Task LoadAudio()
        {
            _audioPlayer = _audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("soft-bell-countdown.wav"));
        }

        private void PlaySound()
        {
            _audioPlayer?.Play();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            RemainingTime--;

            if (RemainingTime == 3)
            {
                PlaySound();
            }

            if (RemainingTime <= 0)
            {
                _timer.Stop();
                IsRunning = false;
                OnSessionCompleted();
            }
        }

        private async void OnSessionCompleted()
        {
            int duration = _currentSessionType == PomodoroSessionType.Work ? _workDuration * 60 : (_currentSessionType == PomodoroSessionType.ShortBreak ? _shortBreakDuration * 60 : _longBreakDuration * 60);
            var session = new PomodoroSession
            {
                StartTime = DateTime.Now.AddSeconds(-duration),
                EndTime = DateTime.Now,
                SessionType = _currentSessionType,
                TaskItemId = SelectedTask?.Id
            };

            await _pomodoroSessionRepository.AddAsync(session);

            if (_currentSessionType == PomodoroSessionType.Work)
            {
                _sessionCount++;
                if (_sessionCount % 4 == 0)
                {
                    _currentSessionType = PomodoroSessionType.LongBreak;
                    RemainingTime = _longBreakDuration * 60;
                    await _notificationService.ShowNotificationAsync("Длинный перерыв", "Время для длительного отдыха!");
                }
                else
                {
                    _currentSessionType = PomodoroSessionType.ShortBreak;
                    RemainingTime = _shortBreakDuration * 60;
                    await _notificationService.ShowNotificationAsync("Короткий перерыв", "Время для короткого перерыва!");
                }
            }
            else
            {
                _currentSessionType = PomodoroSessionType.Work;
                RemainingTime = _workDuration * 60;
                await _notificationService.ShowNotificationAsync("Рабочая сессия", "Время вернуться к работе!");
            }

            if (AutoStartNextSession)
            {
                StartTimer();
            }
        }

        public void StartTimer()
        {
            if (!IsRunning)
            {
                _totalTimeForCurrentSession = RemainingTime;
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

        private void UpdateCommandStates()
        {
            ((Command)StartCommand).ChangeCanExecute();
            ((Command)PauseCommand).ChangeCanExecute();
            ((Command)ResetCommand).ChangeCanExecute();
        }

    }
}
