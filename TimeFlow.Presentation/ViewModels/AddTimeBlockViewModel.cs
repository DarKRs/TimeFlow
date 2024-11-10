using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Presentation.ViewModels
{
    public class AddTimeBlockViewModel : BaseViewModel
    {
        private readonly ITimeBlockService _timeBlockService;
        private readonly ITaskService _taskService;

        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Today;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public ObservableCollection<TimeBlockType> BlockTypes { get; set; }
        public TimeBlockType SelectedBlockType { get; set; }

        public ObservableCollection<TaskItem> Tasks { get; set; }
        public TaskItem SelectedTask { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand BackCommand { get; }

        public AddTimeBlockViewModel(ITimeBlockService timeBlockService, ITaskService taskService)
        {
            _timeBlockService = timeBlockService;
            _taskService = taskService;

            BlockTypes = new ObservableCollection<TimeBlockType>((TimeBlockType[])Enum.GetValues(typeof(TimeBlockType)));
            Tasks = new ObservableCollection<TaskItem>();

            SaveCommand = new Command(async () => await SaveTimeBlock());
            BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

            LoadTasks();
        }

        public async Task LoadTasks() => await LoadTasksAsync(Tasks,_taskService);

        private async Task SaveTimeBlock()
        {
            var startDateTime = StartDate.Add(StartTime);
            var endDateTime = StartDate.Add(EndTime);

            var timeBlock = new TimeBlock
            {
                Title = Title,
                Description = Description,
                StartTime = startDateTime,
                EndTime = endDateTime,
                BlockType = SelectedBlockType,
                TaskItemId = SelectedTask?.Id
            };

            await _timeBlockService.AddTimeBlockAsync(timeBlock);

            // Возврат на страницу планирования
            await Shell.Current.GoToAsync("..");
        }
    }
}
