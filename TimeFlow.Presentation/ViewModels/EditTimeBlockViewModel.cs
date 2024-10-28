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
    public class EditTimeBlockViewModel : BaseViewModel
    {
        private readonly ITimeBlockService _timeBlockService;
        private readonly ITaskService _taskService;

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private DateTime _startTime;
        public DateTime StartTime
        {
            get => _startTime;
            set
            {
                _startTime = value;
                OnPropertyChanged();
            }
        }

        private DateTime _endTime;
        public DateTime EndTime
        {
            get => _endTime;
            set
            {
                _endTime = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<TimeBlockType> BlockTypes { get; set; }
        private TimeBlockType _selectedBlockType;
        public TimeBlockType SelectedBlockType
        {
            get => _selectedBlockType;
            set
            {
                _selectedBlockType = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<TaskItem> Tasks { get; set; }
        private TaskItem _selectedTask;
        public TaskItem SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand BackCommand { get; }

        public EditTimeBlockViewModel(ITimeBlockService timeBlockService, ITaskService taskService)
        {
            _timeBlockService = timeBlockService;
            _taskService = taskService;

            BlockTypes = new ObservableCollection<TimeBlockType>((TimeBlockType[])Enum.GetValues(typeof(TimeBlockType)));
            Tasks = new ObservableCollection<TaskItem>();

            SaveCommand = new Command(async () => await SaveTimeBlock());
            BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        public async void Initialize(TimeBlock timeBlock)
        {
            Title = timeBlock.Title;
            Description = timeBlock.Description;
            StartTime = timeBlock.StartTime;
            EndTime = timeBlock.EndTime;
            SelectedBlockType = timeBlock.BlockType;
            SelectedTask = timeBlock.TaskItem;
            await LoadTasks();
        }

        private async Task LoadTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            Tasks.Clear();
            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }
        }

        private async Task SaveTimeBlock()
        {
            var updatedTimeBlock = new TimeBlock
            {
                Title = this.Title,
                Description = this.Description,
                StartTime = this.StartTime,
                EndTime = this.EndTime,
                BlockType = this.SelectedBlockType,
                TaskItemId = this.SelectedTask?.Id
            };

            await _timeBlockService.UpdateTimeBlockAsync(updatedTimeBlock);
            await Shell.Current.GoToAsync(".."); // Вернуться назад после сохранения
        }
    }
}
