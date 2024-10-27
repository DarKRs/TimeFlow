using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Core.Interfaces;
using TimeFlow.Core.Services;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Presentation.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ITaskService _taskService;

        public ObservableCollection<TaskItem> Tasks { get; set; }


        public MainViewModel(ITaskService taskService)
        {
            _taskService = taskService;
            Tasks = new ObservableCollection<TaskItem>();
            LoadTasks();
        }

        private async void LoadTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }
        }
    }
}
