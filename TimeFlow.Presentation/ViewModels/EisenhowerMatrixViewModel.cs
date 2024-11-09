using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class EisenhowerMatrixViewModel : BaseViewModel
    {
        private readonly ITaskService _taskService;
        private bool _isTaskEditorVisible;
        private string _taskTitle;
        private string _taskDescription;
        private bool _isImportant;
        private bool _isUrgent;
        private DateTime _selectedDate;

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

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        public ObservableCollection<TaskItem> MondayTasks { get; set; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<TaskItem> TuesdayTasks { get; set; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<TaskItem> WednesdayTasks { get; set; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<TaskItem> ThursdayTasks { get; set; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<TaskItem> FridayTasks { get; set; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<TaskItem> SaturdayTasks { get; set; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<TaskItem> SundayTasks { get; set; } = new ObservableCollection<TaskItem>();

        public ICommand AddTaskCommand { get; }
        public ICommand DayTappedCommand { get; }
        public ICommand SaveTaskCommand { get; }
        public ICommand CancelEditCommand { get; }

        public EisenhowerMatrixViewModel(ITaskService taskService)
        {
            _taskService = taskService;

            LoadTasks();
            AddTaskCommand = new Command(async () => await AddTask());
            DayTappedCommand = new Command<string>(async (day) => await OnDayTapped(day));
        }

        public async void LoadTasks()
        {
            var startOfWeek = DateTime.Today.StartOfWeek(DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(7);

            var tasks = await _taskService.GetTasksByDateRangeAsync(startOfWeek, endOfWeek);

            MondayTasks.Clear();
            TuesdayTasks.Clear();
            WednesdayTasks.Clear();
            ThursdayTasks.Clear();
            FridayTasks.Clear();
            SaturdayTasks.Clear();
            SundayTasks.Clear();

            foreach (var task in tasks)
            {
                switch (task.ScheduledDate.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        MondayTasks.Add(task);
                        break;
                    case DayOfWeek.Tuesday:
                        TuesdayTasks.Add(task);
                        break;
                    case DayOfWeek.Wednesday:
                        WednesdayTasks.Add(task);
                        break;
                    case DayOfWeek.Thursday:
                        ThursdayTasks.Add(task);
                        break;
                    case DayOfWeek.Friday:
                        FridayTasks.Add(task);
                        break;
                    case DayOfWeek.Saturday:
                        SaturdayTasks.Add(task);
                        break;
                    case DayOfWeek.Sunday:
                        SundayTasks.Add(task);
                        break;
                }
            }
        }

        private async Task AddTask()
        {
            // Открыть страницу добавления задачи
            await Shell.Current.GoToAsync(nameof(AddTaskPage));
        }

        private async Task OnDayTapped(string day)
        {
            if (Enum.TryParse<DayOfWeek>(day, out var dayOfWeek))
            {
                // Определяем дату выбранного дня недели в текущей неделе
                var selectedDate = DateTime.Today.StartOfWeek(DayOfWeek.Monday).AddDays((int)dayOfWeek - 1);

                await Shell.Current.GoToAsync($"{nameof(AddTaskPage)}?ScheduledDate={selectedDate:yyyy-MM-dd}");
            }
        }

        private async Task SaveTask()
        {
            var newTask = new TaskItem
            {
                Title = TaskTitle,
                Description = TaskDescription,
                ScheduledDate = SelectedDate,
                IsImportant = IsImportant,
                IsUrgent = IsUrgent
            };
            await _taskService.AddTaskAsync(newTask);
            IsTaskEditorVisible = false; // Скрыть редактор после сохранения
        }

        private void CancelEdit()
        {
            IsTaskEditorVisible = false; // Скрыть редактор
            ClearTaskEditor(); // Очистить поля редактора
        }

        public void ShowTaskEditor(DateTime date)
        {
            SelectedDate = date;
            IsTaskEditorVisible = true;
        }

        public void HideTaskEditor()
        {
            IsTaskEditorVisible = false;
        }

        private void ClearTaskEditor()
        {
            TaskTitle = string.Empty;
            TaskDescription = string.Empty;
            IsImportant = false;
            IsUrgent = false;
        }

    }
}
