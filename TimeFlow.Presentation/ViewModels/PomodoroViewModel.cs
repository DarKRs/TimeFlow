using System.Timers;
using System.Windows.Input;
using TimeFlow.Domain.Entities;
using Plugin.LocalNotification;
using Microsoft.Maui.Dispatching;
using Timer = System.Timers.Timer;
using System;
using System.Collections.ObjectModel;
using TimeFlow.Core.Interfaces;
using TimeFlow.Infrastructure.Repositories;
using System.Threading.Tasks;
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
        //
        private Timer _timer;
        private int _remainingTime; // in seconds
        private bool _isRunning;
        private int _totalTimeForCurrentSession;

        private int _workDuration;
        private int _shortBreakDuration;
        private int _longBreakDuration;

        private PomodoroSessionType _currentSessionType;
        private int _sessionCount = 0;

        public ObservableCollection<TaskItem> TodayTasks { get; set; } = new ObservableCollection<TaskItem>();
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

        public PomodoroViewModel(ITaskService taskService, IPomodoroSessionRepository pomodoroSessionRepository)
        {
            _pomodoroSessionRepository = pomodoroSessionRepository;
            _taskService = taskService;

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

            TodayTasks = new ObservableCollection<TaskItem>();
            LoadTodayTasks();
        }

        public async void LoadTodayTasks()
        {
            var todayTasks = await _taskService.GetTasksByDateAsync(DateTime.Today);
            var sorted = todayTasks.OrderBy(task => task.Category);
            TodayTasks.Clear();

            foreach (var task in sorted)
            {
                TodayTasks.Add(task);
            }
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
