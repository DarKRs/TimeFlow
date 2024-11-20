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

        public ICommand DeleteTimeBlockCommand { get; }
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

            DeleteTimeBlockCommand = new Command<TimeBlock>(async (tb) => await DeleteTimeBlock(tb));
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

        public async Task LoadTasks() => await LoadTasksAsync(Tasks, _taskService);

        private async Task DeleteTimeBlock(TimeBlock timeBlock)
        {
            await _timeBlockService.DeleteTimeBlockAsync(timeBlock.Id);
            TimeBlocks.Remove(timeBlock);
        }
        private async Task GenerateTimeBlocks()
        {
            var tasks = await _taskService.GetTasksByDateAsync(DateTime.Today);
            var generatedBlocks = await _timeBlockService.GenerateTimeBlocksForTasksAsync(tasks);

            TimeBlocks.Clear();
            foreach (var block in generatedBlocks)
            {
                TimeBlocks.Add(block);
            }
        }
    }
}
