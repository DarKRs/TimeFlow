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
    public class TimeBlockingViewModel : BaseViewModel
    {
        private readonly ITaskService _taskService;
        private readonly ITimeBlockService _timeBlockService;

        public ObservableCollection<TimeBlock> TimeBlocks { get; set; }
        public ObservableCollection<TaskItem> Tasks { get; set; }

        public ICommand AddTimeBlockCommand { get; }
        public ICommand DeleteTimeBlockCommand { get; }
        public ICommand EditTimeBlockCommand { get; }
        public ICommand GenerateTimeBlocksCommand { get; }

        public List<string> DayNamesInRussian { get; } = new List<string>
        {
            "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс"
        };

        public List<string> MonthNamesInRussian { get; } = new List<string>
        {
            "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь",
            "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"
        };

        public DateTime SelectedDate { get; set; }

        public TimeBlockingViewModel(ITaskService taskService, ITimeBlockService timeBlockService)
        {
            SelectedDate = DateTime.Now;

            _taskService = taskService;
            _timeBlockService = timeBlockService;

            TimeBlocks = new ObservableCollection<TimeBlock>();
            Tasks = new ObservableCollection<TaskItem>();

            AddTimeBlockCommand = new Command(async () => await AddTimeBlock());
            DeleteTimeBlockCommand = new Command<TimeBlock>(async (tb) => await DeleteTimeBlock(tb));
            EditTimeBlockCommand = new Command<TimeBlock>(async (tb) => await EditTimeBlock(tb));
            GenerateTimeBlocksCommand = new Command(async () => await GenerateTimeBlocks());


            LoadData();
        }

        public async void LoadData()
        {
            await LoadTimeBlocks();
            await LoadTasks();
        }

        private async Task LoadTimeBlocks()
        {
            var blocks = await _timeBlockService.GetTimeBlocksForTodayAsync();
            TimeBlocks.Clear();
            foreach (var block in blocks)
            {
                TimeBlocks.Add(block);
            }
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

        private async Task AddTimeBlock()
        {
            // Открыть страницу добавления временного блока
            await Shell.Current.GoToAsync(nameof(AddTimeBlockPage));
        }

        private async Task DeleteTimeBlock(TimeBlock timeBlock)
        {
            await _timeBlockService.DeleteTimeBlockAsync(timeBlock.Id);
            TimeBlocks.Remove(timeBlock);
        }

        private async Task EditTimeBlock(TimeBlock timeBlock)
        {
            // Открыть страницу редактирования временного блока
            var navigationParameter = new Dictionary<string, object>
            {
                { "TimeBlock", timeBlock }
            };
            await Shell.Current.GoToAsync(nameof(EditTimeBlockPage), navigationParameter);
        }

        private async Task GenerateTimeBlocks()
        {
            var tasks = await _taskService.GetTasksForTodayAsync();
            var generatedBlocks = await _timeBlockService.GenerateTimeBlocksForTasksAsync(tasks);

            TimeBlocks.Clear();
            foreach (var block in generatedBlocks)
            {
                TimeBlocks.Add(block);
            }
        }
    }
}
