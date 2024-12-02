﻿using Plugin.LocalNotification;
using Plugin.Maui.Audio;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows.Input;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;
using Timer = System.Timers.Timer;
#if WINDOWS
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
#endif

namespace TimeFlow.Presentation.ViewModels
{
    public class PomodoroViewModel : BaseViewModel
    {
        private readonly IDispatcher _dispatcher;
        private readonly ITaskService _taskService;
        private readonly IPomodoroSessionRepository _pomodoroSessionRepository;
        private readonly IAudioManager _audioManager;
        private IAudioPlayer _audioPlayer;
        //
        private Timer _timer;
        private int _remainingTime; // in seconds
        private bool _isRunning;
        private int _totalTimeForCurrentSession;

        private PomodoroSessionType _currentSessionType;
        private int _sessionCount = 0;

        public Grid TabPanel { get; set; }
        private ObservableCollection<TaskItem> _todayTasks;
        public ObservableCollection<TaskItem> TodayTasks
        {
            get => _todayTasks;
            set
            {
                if (_todayTasks != value)
                {
                    _todayTasks = value;
                    OnPropertyChanged(nameof(TodayTasks));
                }
            }
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
                    if (_currentSessionType == PomodoroSessionType.Work)
                    {
                        RemainingTime = WorkDuration * 60;
                        OnPropertyChanged(nameof(TimeDisplay));
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
                    if (_currentSessionType == PomodoroSessionType.ShortBreak)
                    {
                        RemainingTime = ShortBreakDuration * 60;
                        OnPropertyChanged(nameof(TimeDisplay));
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
                    if (_currentSessionType == PomodoroSessionType.LongBreak)
                    {
                        RemainingTime = LongBreakDuration * 60;
                        OnPropertyChanged(nameof(TimeDisplay));
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
                    _remainingTime = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TimeDisplay));
                    OnPropertyChanged(nameof(Progress));
                    OnPropertyChanged(nameof(CurrentSessionTypeDisplay));
                }
            }
        }

        public string CurrentSessionTypeDisplay
        {
            get
            {
                return _currentSessionType switch
                {
                    PomodoroSessionType.Work => "Рабочий интервал",
                    PomodoroSessionType.ShortBreak => "Короткий перерыв",
                    PomodoroSessionType.LongBreak => "Длинный перерыв",
                    _ => "Неизвестный статус"
                };
            }
        }

        private bool _autoStartNextSession = true;

        public bool AutoStartNextSession
        {
            get => _autoStartNextSession;
            set => SetProperty(ref _autoStartNextSession, value);
        }

        // Прогресс интервала
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
        public ICommand SaveSettingsCommand { get; }
        public ICommand TogglePanelCommand { get; }
        public ICommand ShowTasksTabCommand { get; }
        public ICommand ShowSettingsTabCommand { get; }

        public PomodoroViewModel(ITaskService taskService, IPomodoroSessionRepository pomodoroSessionRepository, IAudioManager audioManager)
        {
            _pomodoroSessionRepository = pomodoroSessionRepository;
            _taskService = taskService;
            _audioManager = audioManager;

#if DEBUG
            // Уменьшенные значения для отладки
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

            _timer = new Timer(1000);
            _timer.Elapsed += OnTimerElapsed;

            StartCommand = new Command(StartTimer, () => CanStart);
            PauseCommand = new Command(PauseTimer, () => CanPause);
            ResetCommand = new Command(ResetTimer, () => CanReset);
            TogglePanelCommand = new Command(TogglePanelAsync);
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
            var sortedTasks = todayTasks.OrderBy(task => task.Category);

            TodayTasks = new ObservableCollection<TaskItem>(sortedTasks);
        }

        public async Task UpdateTaskCompletionStatus(TaskItem task)
        {
            if (task != null)
            {
                await _taskService.UpdateTaskAsync(task); 
                OnPropertyChanged(nameof(TodayTasks));
            }
        }

        private async void LoadAudio()
        {
            _audioPlayer = _audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("soft-bell-countdown.wav"));
        }

        private void PlaySound()
        {
            _audioPlayer?.Play();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
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

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    OnSessionCompleted();
                });
            }
        }

        private async void OnSessionCompleted()
        {
            var session = new PomodoroSession
            {
                StartTime = DateTime.Now.AddSeconds(-(_currentSessionType == PomodoroSessionType.Work ? _workDuration * 60 : GetCurrentBreakDuration() * 60)),
                EndTime = DateTime.Now,
                SessionType = _currentSessionType,
                TaskItemId = SelectedTask?.Id
            };

            await _pomodoroSessionRepository.AddSessionAsync(session);

            if (_currentSessionType == PomodoroSessionType.Work)
            {
                _sessionCount++;
                if (_sessionCount % 4 == 0)
                {
                    _currentSessionType = PomodoroSessionType.LongBreak;
                    RemainingTime = _longBreakDuration * 60;
                    ShowNotification("Длинный перерыв", "Время для длительного отдыха!");
                }
                else
                {
                    _currentSessionType = PomodoroSessionType.ShortBreak;
                    RemainingTime = _shortBreakDuration * 60;
                    ShowNotification("Короткий перерыв", "Время для короткого перерыва!");
                }
            }
            else
            {
                _currentSessionType = PomodoroSessionType.Work;
                RemainingTime = _workDuration * 60;
                ShowNotification("Рабочая сессия", "Время вернуться к работе!");
            }

            if (AutoStartNextSession)
            {
                StartTimer();
            }
        }

        private int GetCurrentBreakDuration()
        {
            return _currentSessionType == PomodoroSessionType.ShortBreak ? _shortBreakDuration : _longBreakDuration;
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


        private async void TogglePanelAsync()
        {
            if (!IsPanelVisible)
            {
                TabPanel.TranslationX = 420; // Установка стартового положения перед первым показом
                IsPanelVisible = true;
                await TabPanel.TranslateTo(0, 0, 350, Easing.SinInOut);
            }
            else
            {
                await TabPanel.TranslateTo(420, 0, 350, Easing.SinInOut);
                IsPanelVisible = false;
            }
        }



        private async void ShowNotification(string title, string message)
        {
#if ANDROID || IOS
            // Уведомления для Android/iOS через Plugin.LocalNotification
            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }

            var notification = new NotificationRequest
            {
                NotificationId = 100,
                Title = title,
                Description = message,
                ReturningData = "PomodoroFinished",
                Schedule =
                    {
                        NotifyTime = DateTime.Now.AddSeconds(5)
                    }
            };
            await LocalNotificationCenter.Current.Show(notification);
#elif WINDOWS
                // Уведомления для Windows
                ShowWindowsToastNotification(title, message);
#endif
        }

        private void ShowWindowsToastNotification(string title, string message)
        {
#if WINDOWS
               var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

                var stringElements = toastXml.GetElementsByTagName("text");
                stringElements[0].AppendChild(toastXml.CreateTextNode(title));
                stringElements[1].AppendChild(toastXml.CreateTextNode(message));

                var toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier("PomodoroApp").Show(toast);
#endif
        }
    }
}
