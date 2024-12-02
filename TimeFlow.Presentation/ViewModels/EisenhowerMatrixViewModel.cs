using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;
using TimeFlow.Presentation.Utils;
using TimeFlow.Presentation.Views;

namespace TimeFlow.Presentation.ViewModels
{
    public class DayTasks
    {
        public string DayName { get; set; }
        public DateTime Date { get; set; }
        public ObservableCollection<TaskItem> Tasks { get; set; }
    }

    public class EisenhowerMatrixViewModel : BaseViewModel
    {
        private readonly ITaskService _taskService;
        private bool _isTaskEditorVisible;
        private string _taskTitle;
        private string _taskDescription;
        private bool _isImportant;
        private bool _isUrgent;
        private DateTime _selectedStartDate;
        private DateTime _selectedEndDate;
        private TimeSpan _plannedStartTime = new TimeSpan(9, 0, 0); // Начало по умолчанию - 9:00
        private TimeSpan _estimatedDuration = new TimeSpan(2, 0, 0); // Длительность по умолчанию - 2 часа

        public bool IsTaskEditorVisible
        {
            get => _isTaskEditorVisible;
            set => SetProperty(ref _isTaskEditorVisible, value);
        }

        public string TaskTitle
        {
            get => _taskTitle;
            set => SetProperty(ref _taskTitle, value);
        }

        public string TaskDescription
        {
            get => _taskDescription;
            set => SetProperty(ref _taskDescription, value);
        }

        public bool IsImportant
        {
            get => _isImportant;
            set => SetProperty(ref _isImportant, value);
        }

        public bool IsUrgent
        {
            get => _isUrgent;
            set => SetProperty(ref _isUrgent, value);
        }

        public DateTime SelectedStartDate
        {
            get => _selectedStartDate;
            set
            {
                SetProperty(ref _selectedStartDate, value);
                OnPropertyChanged(nameof(DisplayedDateText));
            }
        }

        public DateTime SelectedEndDate
        {
            get => _selectedEndDate;
            set
            {
                SetProperty(ref _selectedEndDate, value);
                OnPropertyChanged(nameof(DisplayedDateText));
            }
        }

        public TimeSpan PlannedStartTime
        {
            get => _plannedStartTime;
            set => SetProperty(ref _plannedStartTime, value);
        }

        public TimeSpan EstimatedDuration
        {
            get => _estimatedDuration;
            set => SetProperty(ref _estimatedDuration, value);
        }

        public string DisplayedDateText => SelectedStartDate == SelectedEndDate
                        ? $"Дата: {SelectedStartDate:dd MMMM yyyy}"
                        : $"Диапазон: {SelectedStartDate:dd MMMM yyyy} - {SelectedEndDate:dd MMMM yyyy}";

        public ObservableCollection<DayTasks> WeekTasks { get; set; } = new ObservableCollection<DayTasks>();

        public ICommand DayTappedCommand { get; }
        public ICommand SaveTaskCommand { get; }
        public ICommand CancelEditCommand { get; }

        public EisenhowerMatrixViewModel(ITaskService taskService)
        {
            _taskService = taskService;

            LoadTasks();
            DayTappedCommand = new Command<string>(async (day) => await OnDayTapped(day));
            SaveTaskCommand = new Command(async () => await SaveTask());
            CancelEditCommand = new Command(async () => await CancelEdit());
        }

        public async void LoadTasks()
        {
            var startOfWeek = DateTime.Today.StartOfWeek(DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(7);
            var tasks = await _taskService.GetTasksByDateRangeAsync(startOfWeek, endOfWeek);

            WeekTasks.Clear();
            var cultureInfo = new System.Globalization.CultureInfo("ru-RU");
            for (int i = 0; i < 7; i++)
            {
                var currentDay = startOfWeek.AddDays(i);
                var dayTasks = tasks.Where(task => task.ScheduledDate.Date == currentDay.Date);
                var dayName = currentDay.ToString("dddd", cultureInfo);
                dayName = cultureInfo.TextInfo.ToTitleCase(dayName);

                WeekTasks.Add(new DayTasks
                {
                    DayName = dayName,
                    Date = currentDay,
                    Tasks = new ObservableCollection<TaskItem>(dayTasks)
                });
            }
        }

        public async Task UpdateTaskCompletionStatus(TaskItem task)
        {
            if (task != null)
            {
                await _taskService.UpdateTaskAsync(task);
                OnPropertyChanged(nameof(WeekTasks)); 
            }
        }

        private async Task OnDayTapped(string day)
        {
            if (Enum.TryParse<DayOfWeek>(day, out var dayOfWeek))
            {
                // Определяем дату выбранного дня недели в текущей неделе
                var selectedDate = DateTime.Today.StartOfWeek(DayOfWeek.Monday).AddDays((int)dayOfWeek - 1);

               // await Shell.Current.GoToAsync($"{nameof(AddTaskPage)}?ScheduledDate={selectedDate:yyyy-MM-dd}");
            }
        }

        private async Task SaveTask()
        {
            var startDate = SelectedStartDate;
            var endDate = SelectedEndDate;

            if (startDate == endDate)
            {
                var newTask = new TaskItem
                {
                    Title = TaskTitle,
                    Description = TaskDescription,
                    ScheduledDate = startDate,
                    PlannedStart = startDate.Add(PlannedStartTime),
                    PlannedEnd = startDate.Add(PlannedStartTime).Add(EstimatedDuration),
                    EstimatedDuration = EstimatedDuration,
                    IsImportant = IsImportant,
                    IsUrgent = IsUrgent
                };
                await _taskService.AddTaskAsync(newTask);
            }
            else // Если выбран диапазон
            {
                var currentDate = startDate;
                while (currentDate <= endDate)
                {
                    var newTask = new TaskItem
                    {
                        Title = TaskTitle,
                        Description = TaskDescription,
                        ScheduledDate = currentDate,
                        PlannedStart = startDate.Add(PlannedStartTime),
                        PlannedEnd = startDate.Add(PlannedStartTime).Add(EstimatedDuration),
                        EstimatedDuration = EstimatedDuration,
                        IsImportant = IsImportant,
                        IsUrgent = IsUrgent
                    };
                    await _taskService.AddTaskAsync(newTask);
                    currentDate = currentDate.AddDays(1); 
                }
            }



            IsTaskEditorVisible = false;
            ClearTaskEditor();
            LoadTasks();
        }
        private async Task CancelEdit()
        {
            IsTaskEditorVisible = false; 
            ClearTaskEditor(); 
        }
        public void ClearTaskEditor()
        {
            PlannedStartTime = new TimeSpan(9, 0, 0); //Сброс времени
            EstimatedDuration = new TimeSpan(2, 0, 0);
            TaskTitle = string.Empty;
            TaskDescription = string.Empty;
            IsImportant = false;
            IsUrgent = false;
        }


    }
}
