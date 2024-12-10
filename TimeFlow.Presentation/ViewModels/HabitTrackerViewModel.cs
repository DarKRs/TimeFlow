using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Presentation.ViewModels
{
    public class HabitTrackerViewModel : BaseViewModel
    {
        private readonly IHabitService _habitService;

        private DateTime _currentMonth;
        public string CurrentMonthYear
        {
            get
            {
                var cultureInfo = CultureInfo.GetCultureInfo("ru-RU");
                return cultureInfo.TextInfo.ToTitleCase(_currentMonth.ToString("MMMM yyyy", cultureInfo));
            }
        }

        public ObservableCollection<Habit> Habits { get; set; } = new();

        public DateTime OldestDate { get; private set; }
        public DateTime NewestDate { get; private set; }

        public List<DateTime> CurrentMonthDates
        {
            get
            {
                var lastDayOfCurrentMonth = _currentMonth;
                var startDate = lastDayOfCurrentMonth.AddDays(-30); // За 31 день

                return Enumerable.Range(0, 31)
                                 .Select(dayOffset => startDate.AddDays(dayOffset))
                                 .ToList();
            }
        }

        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand ToggleHabitStatusCommand { get; }


        public HabitTrackerViewModel(IHabitService habitService)
        {
            _habitService = habitService;
            _currentMonth = DateTime.Now;

            PreviousMonthCommand = new Command(() => ChangeMonth(-1), () => CanNavigateToPreviousMonth);
            NextMonthCommand = new Command(() => ChangeMonth(1), () => CanNavigateToNextMonth);
            ToggleHabitStatusCommand = new Command<HabitRecord>(async (record) => await ToggleHabitStatus(record));

            LoadHabitsForMonth(DateTime.Now.Year, DateTime.Now.Month);
            UpdateDateLimits();
        }

        private void ChangeMonth(int offset)
        {
            _currentMonth = _currentMonth.AddMonths(offset);
            OnPropertyChanged(nameof(CurrentMonthYear));
            OnPropertyChanged(nameof(CurrentMonthDates));
            UpdateCommandStates();
        }

        private bool CanNavigateToPreviousMonth => _currentMonth > OldestDate;
        private bool CanNavigateToNextMonth => _currentMonth < NewestDate;

        private void UpdateCommandStates()
        {
            ((Command)PreviousMonthCommand).ChangeCanExecute();
            ((Command)NextMonthCommand).ChangeCanExecute();
        }

        private void UpdateDateLimits()
        {
            if (Habits.Any())
            {
                OldestDate = Habits.Min(h => h.CreatedDate); 
                NewestDate = DateTime.Today; 
            }
            else
            {
                OldestDate = DateTime.Today.AddMonths(-1);
                NewestDate = DateTime.Today;
            }
        }

        private async void LoadHabitsForMonth(int year, int month)
        {
            var habits = await _habitService.GetHabitsForMonth(year, month);
            Habits.Clear();

            Habits.Add(new Habit
            {
                Id = 1,
                Name = "Пить воду",
                Description = "Выпивать 8 стаканов воды в день",
                CompletionRecords = Enumerable.Range(1, 31).Select(i => new HabitRecord
                {
                    Date = new DateTime(year, month, i),
                    Status = i % 2 == 0 ? CompletionStatus.Done : CompletionStatus.NotDone
                }).ToList()
            });

            Habits.Add(new Habit
            {
                Id = 2,
                Name = "Читать книги",
                Description = "Читать 10 страниц каждый день",
                CompletionRecords = Enumerable.Range(1, 31).Select(i => new HabitRecord
                {
                    Date = new DateTime(year, month, i),
                    Status = i % 3 == 0 ? CompletionStatus.PartiallyDone : CompletionStatus.Done
                }).ToList()
            });

            Habits.Add(new Habit
            {
                Id = 3,
                Name = "Физическая активность",
                Description = "15 минут утренней зарядки",
                CompletionRecords = Enumerable.Range(1, 31).Select(i => new HabitRecord
                {
                    Date = new DateTime(year, month, i),
                    Status = i % 5 == 0 ? CompletionStatus.NotDone : CompletionStatus.Done
                }).ToList()
            });

            foreach (var habit in habits)
            {
                Habits.Add(habit);
            }
        }

        private async Task ToggleHabitStatus(HabitRecord record)
        {
            record.Status = record.Status == CompletionStatus.Done ? CompletionStatus.NotDone : CompletionStatus.Done;
            await _habitService.UpdateHabitRecordAsync(record);
        }

        private async void LoadHabits()
        {
            var habits = await _habitService.GetHabitsForMonth(_currentMonth.Year, _currentMonth.Month);
            Habits.Clear();
            foreach (var habit in habits)
            {
                Habits.Add(habit);
            }

            //Тестовые привычки
            Habits.Add(new Habit
            {
                Id = 1,
                Name = "Пить воду",
                Description = "Выпивать 8 стаканов воды в день",
                Periodicity = new HabitPeriodicity
                {
                    FrequencyType = Frequency.Daily,
                },
                CompletionRecords = new List<HabitRecord>
                {
                    new HabitRecord { Date = DateTime.Today.AddDays(-2), Status = CompletionStatus.Done },
                    new HabitRecord { Date = DateTime.Today.AddDays(-1), Status = CompletionStatus.PartiallyDone },
                    new HabitRecord { Date = DateTime.Today, Status = CompletionStatus.NotDone }
                },
                CurrentStreak = 2,
                LongestStreak = 5,
                LastCompletionDate = DateTime.Today.AddDays(-1),
                CreatedDate = DateTime.Today.AddDays(-30)
            });

            Habits.Add(new Habit
            {
                Id = 2,
                Name = "Чтение книг",
                Description = "Читать 10 страниц книги каждый день",
                Periodicity = new HabitPeriodicity
                {
                    FrequencyType = Frequency.Daily,
                },
                CompletionRecords = new List<HabitRecord>
                {
                    new HabitRecord { Date = DateTime.Today.AddDays(-3), Status = CompletionStatus.Done },
                    new HabitRecord { Date = DateTime.Today.AddDays(-2), Status = CompletionStatus.Done },
                    new HabitRecord { Date = DateTime.Today.AddDays(-1), Status = CompletionStatus.PartiallyDone },
                    new HabitRecord { Date = DateTime.Today, Status = CompletionStatus.Done }
                },
                CurrentStreak = 3,
                LongestStreak = 7,
                LastCompletionDate = DateTime.Today,
                CreatedDate = DateTime.Today.AddDays(-60)
            });

            Habits.Add(new Habit
            {
                Id = 3,
                Name = "Физические упражнения",
                Description = "15 минут зарядки каждое утро",
                Periodicity = new HabitPeriodicity
                {
                    FrequencyType = Frequency.Daily,
                },
                CompletionRecords = new List<HabitRecord>
                {
                    new HabitRecord { Date = DateTime.Today.AddDays(-4), Status = CompletionStatus.NotDone },
                    new HabitRecord { Date = DateTime.Today.AddDays(-3), Status = CompletionStatus.Done },
                    new HabitRecord { Date = DateTime.Today.AddDays(-2), Status = CompletionStatus.PartiallyDone },
                    new HabitRecord { Date = DateTime.Today.AddDays(-1), Status = CompletionStatus.Done }
                },
                CurrentStreak = 1,
                LongestStreak = 4,
                LastCompletionDate = DateTime.Today.AddDays(-1),
                CreatedDate = DateTime.Today.AddDays(-15)
            });
        }
    }
}
