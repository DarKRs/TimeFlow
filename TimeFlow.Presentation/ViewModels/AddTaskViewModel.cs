using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Presentation.ViewModels
{
    public class AddTaskViewModel : BaseViewModel
    {
        private readonly ITaskService _taskService;

        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsImportant { get; set; }
        public bool IsUrgent { get; set; }

        public ICommand SaveCommand { get; }

        public AddTaskViewModel(ITaskService taskService)
        {
            _taskService = taskService;
            SaveCommand = new Command(async () => await SaveTask());
        }

        private async Task SaveTask()
        {
            var task = new TaskItem
            {
                Title = Title,
                Description = Description,
                IsImportant = IsImportant,
                IsUrgent = IsUrgent
            };

            await _taskService.AddTaskAsync(task);

            // Возврат на страницу Матрицы
            await Shell.Current.GoToAsync("..");
        }
    }
}
