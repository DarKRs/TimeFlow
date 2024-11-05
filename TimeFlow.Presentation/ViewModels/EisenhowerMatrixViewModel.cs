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

        public ObservableCollection<TaskItem> MondayTasks { get; set; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<TaskItem> TuesdayTasks { get; set; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<TaskItem> WednesdayTasks { get; set; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<TaskItem> ThursdayTasks { get; set; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<TaskItem> FridayTasks { get; set; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<TaskItem> SaturdayTasks { get; set; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<TaskItem> SundayTasks { get; set; } = new ObservableCollection<TaskItem>();

        public ICommand AddTaskCommand { get; }
        public ICommand DayTappedCommand { get; }

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

    }
}
