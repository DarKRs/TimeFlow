using System.Collections.ObjectModel;
using System.Windows.Input;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;
using TimeFlow.Presentation.AdditionalModels;
using TimeFlow.Presentation.Utils;

namespace TimeFlow.Presentation.ViewModels
{
    public class EisenhowerMatrixViewModel : BaseViewModel
    {
        private readonly ITaskService _taskService;
        private readonly IDispatcher _dispatcher;

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
                if (SetProperty(ref _selectedStartDate, value))
                {
                    OnPropertyChanged(nameof(DisplayedDateText));
                }
            }
        }

        public DateTime SelectedEndDate
        {
            get => _selectedEndDate;
            set
            {
                if (SetProperty(ref _selectedEndDate, value))
                {
                    OnPropertyChanged(nameof(DisplayedDateText));
                }
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

        public ICommand SaveTaskCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand DeleteTaskCommand { get; }

        public EisenhowerMatrixViewModel(ITaskService taskService)
        {
            _taskService = taskService;
            _dispatcher = Dispatcher.GetForCurrentThread();

            LoadTasksAsync();
            SaveTaskCommand = new Command(async () => await SaveTaskAsync());
            DeleteTaskCommand = new Command<TaskItem>(async (task) => await DeleteTaskAsync(task));

            CancelEditCommand = new Command(CancelEdit);
        }

        public async void LoadTasksAsync()
        {
            var startOfWeek = DateTime.Today.StartOfWeek(DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(6); // 7 дней, включая начало недели
            var tasks = await _taskService.GetTasksByDateRangeAsync(startOfWeek, endOfWeek);

            var cultureInfo = new System.Globalization.CultureInfo("ru-RU");

            _dispatcher.Dispatch(() =>
            {
                WeekTasks.Clear();
                for (int i = 0; i < 7; i++)
                {
                    var currentDay = startOfWeek.AddDays(i);
                    var dayTasks = tasks.Where(task => task.ScheduledDate.Date == currentDay.Date);
                    var dayName = cultureInfo.TextInfo.ToTitleCase(currentDay.ToString("dddd", cultureInfo));

                    WeekTasks.Add(new DayTasks
                    {
                        DayName = dayName,
                        Date = currentDay,
                        Tasks = new ObservableCollection<TaskItem>(dayTasks)
                    });
                }
            });
        }

        public async Task UpdateTaskCompletionStatus(TaskItem task)
        {
            if (task != null)
            {
                await _taskService.UpdateTaskAsync(task);
            }
        }

        private async Task DeleteTaskAsync(TaskItem task)
        {
            if (task != null)
            {
                await _taskService.DeleteTaskAsync(task.Id);
                LoadTasksAsync();
            }
        }

        private async Task SaveTaskAsync()
        {
            var startDate = SelectedStartDate;
            var endDate = SelectedEndDate;

            var tasksToAdd = new List<TaskItem>();

            var currentDate = startDate;
            while (currentDate <= endDate)
            {
                var newTask = new TaskItem
                {
                    Title = TaskTitle,
                    Description = TaskDescription,
                    ScheduledDate = currentDate,
                    PlannedStart = currentDate.Add(PlannedStartTime),
                    PlannedEnd = currentDate.Add(PlannedStartTime).Add(EstimatedDuration),
                    EstimatedDuration = EstimatedDuration,
                    IsImportant = IsImportant,
                    IsUrgent = IsUrgent
                };
                tasksToAdd.Add(newTask);
                currentDate = currentDate.AddDays(1);
            }

            foreach (var task in tasksToAdd)
            {
                await _taskService.AddTaskAsync(task);
            }

            IsTaskEditorVisible = false;
            ClearTaskEditor();
            LoadTasksAsync();
        }

        private void CancelEdit()
        {
            IsTaskEditorVisible = false;
            ClearTaskEditor();
        }

        public void ClearTaskEditor()
        {
            PlannedStartTime = new TimeSpan(9, 0, 0); // Сброс времени
            EstimatedDuration = new TimeSpan(2, 0, 0);
            TaskTitle = string.Empty;
            TaskDescription = string.Empty;
            IsImportant = false;
            IsUrgent = false;
            SelectedStartDate = DateTime.Today;
            SelectedEndDate = DateTime.Today;
        }
    }
}
