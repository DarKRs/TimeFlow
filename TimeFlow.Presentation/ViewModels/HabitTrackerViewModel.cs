using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;
using TimeFlow.Presentation.AdditionalModels.DTO;
using TimeFlow.Presentation.ViewModels.Popups;
using TimeFlow.Presentation.Views.Popups;

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

        public ObservableCollection<HabitDTO> Habits { get; set; } = new();

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

                var allDates = Enumerable.Range(0, daysInMonth)
                                         .Select(d => firstDayOfMonth.AddDays(d))
                                         .ToList();

                if (year == DateTime.Today.Year && month == DateTime.Today.Month)
                    allDates = allDates.Where(d => d <= DateTime.Today).ToList();

                return allDates;
            }
        }

        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand ShowAddHabitPopupCommand { get; }
        public ICommand ToggleHabitStatusCommand { get; }

        public HabitTrackerViewModel(IHabitService habitService)
        {
            _habitService = habitService;
            _currentMonth = DateTime.Now;

            PreviousMonthCommand = new Command(() => ChangeMonth(-1), () => CanNavigateToPreviousMonth);
            NextMonthCommand = new Command(() => ChangeMonth(1), () => CanNavigateToNextMonth);
            ToggleHabitStatusCommand = new Command<HabitRecordDTO>(async (record) => await ToggleHabitStatus(record));
            ShowAddHabitPopupCommand = new Command(() => ShowAddHabitPopup());

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

            var newHabits = new ObservableCollection<HabitDTO>();

            foreach (var habit in loadedHabits)
            {
                // Преобразуем в словарь для быстрого доступа
                var recordsByDate = habit.CompletionRecords?
                    .ToDictionary(r => r.Date.Date, r => r)
                    ?? new Dictionary<DateTime, HabitRecord>();

                var displayedRecords = new ObservableCollection<HabitRecordDTO>();

                foreach (var date in dates)
                {
                    if (recordsByDate.TryGetValue(date.Date, out var existingRecord))
                    {
                        displayedRecords.Add(new HabitRecordDTO
                        {
                            Id = existingRecord.Id,
                            HabitId = habit.Id,
                            Date = existingRecord.Date,
                            Status = existingRecord.Status
                        });
                    }
                    else
                    {
                        var status = date.Date < habit.CreatedDate.Date
                            ? CompletionStatus.NotApplicable
                            : CompletionStatus.NotDone;

                        displayedRecords.Add(new HabitRecordDTO
                        {
                            HabitId = habit.Id,
                            Date = date.Date,
                            Status = status
                        });
                    }
                }

                // Мапим Habit -> HabitDTO
                var habitDTO = new HabitDTO
                {
                    Id = habit.Id,
                    Name = habit.Name,
                    Description = habit.Description,
                    CreatedDate = habit.CreatedDate,
                    IsActive = habit.IsActive,
                    AllowedMissedDays = habit.AllowedMissedDays,
                    DisplayedRecords = displayedRecords
                };

                newHabits.Add(habitDTO);
            }

            Habits = newHabits;
            OnPropertyChanged(nameof(Habits));
        }

        private async Task ToggleHabitStatus(HabitRecordDTO record)
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


            if (record.Id == 0 && record.Status != CompletionStatus.NotApplicable)
            {
                var newRecord = new HabitRecord
                {
                    HabitId = record.HabitId,
                    Date = record.Date,
                    Status = record.Status
                };

                var newId = await _habitService.AddHabitRecordAsync(newRecord);
                record.Id = newId; // присваиваем DTO-шке Id из БД
            }
            else
            {
                if (record.Id > 0)
                {
                    await _habitService.UpdateHabitRecordAsync(new HabitRecord
                    {
                        Id = record.Id,
                        HabitId = record.HabitId,
                        Date = record.Date,
                        Status = record.Status
                    });
                }
            }


            OnPropertyChanged(nameof(Habits));
        }

        private void ShowAddHabitPopup()
        {
            var popupView = new AddHabitPopup(new AddHabitPopupViewModel(_habitService));
            Application.Current.MainPage.ShowPopup(popupView);
        }
    }

}
