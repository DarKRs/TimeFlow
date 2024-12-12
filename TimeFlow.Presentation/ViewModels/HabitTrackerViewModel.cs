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
                var year = _currentMonth.Year;
                var month = _currentMonth.Month;
                var firstDayOfMonth = new DateTime(year, month, 1);
                var daysInMonth = DateTime.DaysInMonth(year, month);

                // Формируем список дат текущего месяца
                var allDates = Enumerable.Range(0, daysInMonth)
                                         .Select(d => firstDayOfMonth.AddDays(d))
                                 .ToList();

                // Если это текущий месяц, обрезаем будущие дни
                if (year == DateTime.Today.Year && month == DateTime.Today.Month)
                    allDates = allDates.Where(d => d <= DateTime.Today).ToList();

                return allDates;
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
            LoadHabitsForMonth(_currentMonth.Year, _currentMonth.Month);
            UpdateDateLimits();
            OnPropertyChanged(nameof(Habits)); 
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
            var loadedHabits = await _habitService.GetAllHabitsAsync();
            var dates = CurrentMonthDates;

            var newHabits = new ObservableCollection<Habit>();

            foreach (var habit in loadedHabits)
            {
                if (habit.CompletionRecords == null)
                    habit.CompletionRecords = new ObservableCollection<HabitRecord>();

                // Преобразуем в словарь для быстрого доступа
                var recordsByDate = habit.CompletionRecords
                    .ToDictionary(r => r.Date.Date, r => r);

                var displayedRecords = new ObservableCollection<HabitRecord>();

                foreach (var date in dates)
                {
                    if (recordsByDate.TryGetValue(date.Date, out var existingRecord))
                    {
                        displayedRecords.Add(existingRecord);
                    }
                    else
                    {
                        // Создаём новую запись
                        var status = date.Date < habit.CreatedDate.Date
                            ? CompletionStatus.NotApplicable 
                            : CompletionStatus.NotDone;

                        displayedRecords.Add(new HabitRecord
                        {
                            Date = date.Date,
                            Status = status,
                            HabitId = habit.Id
                        });
                    }
                }

                habit.CompletionRecords = displayedRecords;
                newHabits.Add(habit);
            }

            Habits = newHabits;
            OnPropertyChanged(nameof(Habits));
        }

        private async Task ToggleHabitStatus(HabitRecord record)
        {
            switch (record.Status)
            {
                case CompletionStatus.NotDone:
                    record.Status = CompletionStatus.PartiallyDone;
                    break;
                case CompletionStatus.PartiallyDone:
                    record.Status = CompletionStatus.Done;
                    break;
                case CompletionStatus.Done:
                    record.Status = CompletionStatus.NotDone;
                    break;
            }

            await _habitService.UpdateHabitRecordAsync(record);
        }

            OnPropertyChanged(nameof(Habits));
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
