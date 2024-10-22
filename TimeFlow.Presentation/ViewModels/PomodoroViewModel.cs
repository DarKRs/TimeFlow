using System.Timers;
using System.Windows.Input;
using TimeFlow.Domain.Entities;
using Plugin.LocalNotification;
using Microsoft.Maui.Dispatching;
using Timer = System.Timers.Timer;
using System;
#if WINDOWS
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
#endif

namespace TimeFlow.Presentation.ViewModels
{
    public class PomodoroViewModel : BaseViewModel
    {
        private readonly IDispatcher _dispatcher;
        private Timer _timer;
        private int _remainingTime; // in seconds
        private bool _isRunning;
        private int _totalTimeForCurrentSession;

        private int _workDuration;
        private int _shortBreakDuration;
        private int _longBreakDuration;

        private PomodoroSessionType _currentSessionType;
        private int _sessionCount = 0;

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
            set
            {
                _autoStartNextSession = value;
                OnPropertyChanged();
            }
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

        public PomodoroViewModel()
        {
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
