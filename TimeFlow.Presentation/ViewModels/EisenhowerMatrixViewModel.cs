using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;
using TimeFlow.Presentation.Views;

namespace TimeFlow.Presentation.ViewModels
{
    public class EisenhowerMatrixViewModel : BaseViewModel
    {
        private readonly ITaskService _taskService;

        public ObservableCollection<TaskItem> UrgentImportantTasks { get; set; }
        public ObservableCollection<TaskItem> NotUrgentImportantTasks { get; set; }
        public ObservableCollection<TaskItem> UrgentNotImportantTasks { get; set; }
        public ObservableCollection<TaskItem> NotUrgentNotImportantTasks { get; set; }

        public ICommand AddTaskCommand { get; }

        public EisenhowerMatrixViewModel(ITaskService taskService)
        {
            _taskService = taskService;

            UrgentImportantTasks = new ObservableCollection<TaskItem>();
            NotUrgentImportantTasks = new ObservableCollection<TaskItem>();
            UrgentNotImportantTasks = new ObservableCollection<TaskItem>();
            NotUrgentNotImportantTasks = new ObservableCollection<TaskItem>();

            LoadTasks();
            AddTaskCommand = new Command(async () => await AddTask());
        }

        public async void LoadTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();

            UrgentImportantTasks.Clear();
            NotUrgentImportantTasks.Clear();
            UrgentNotImportantTasks.Clear();
            NotUrgentNotImportantTasks.Clear();

            foreach (var task in tasks)
            {
                switch (task.Category)
                {
                    case TaskCategory.UrgentImportant:
                        UrgentImportantTasks.Add(task);
                        break;
                    case TaskCategory.NotUrgentImportant:
                        NotUrgentImportantTasks.Add(task);
                        break;
                    case TaskCategory.UrgentNotImportant:
                        UrgentNotImportantTasks.Add(task);
                        break;
                    case TaskCategory.NotUrgentNotImportant:
                        NotUrgentNotImportantTasks.Add(task);
                        break;
                }
            }
        }

        private async Task AddTask()
        {
            // Открыть страницу добавления задачи
            await Shell.Current.GoToAsync(nameof(AddTaskPage));
        }

        
    }
}
