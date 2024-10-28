using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Core.Services
{
    public class TimeBlockService : ITimeBlockService
    {
        private readonly ITimeBlockRepository _timeBlockRepository;

        public TimeBlockService(ITimeBlockRepository timeBlockRepository)
        {
            _timeBlockRepository = timeBlockRepository;
        }

        public async Task AddTimeBlockAsync(TimeBlock block)
        {
           await _timeBlockRepository.AddTimeBlockAsync(block);
        }

        public async Task DeleteTimeBlockAsync(int id)
        {
            await _timeBlockRepository.DeleteTimeBlockAsync(id);
        }

        public async Task<IEnumerable<TimeBlock>> GetTimeBlocksForTodayAsync()
        {
            return await _timeBlockRepository.GetTimeBlocksByDateAsync(DateTime.Today);
        }

        public async Task<IEnumerable<TimeBlock>> GetTimeBlocksByDateAsync(DateTime date)
        {
            return await _timeBlockRepository.GetTimeBlocksByDateAsync(date);
        }

        public async Task UpdateTimeBlockAsync(TimeBlock block)
        {
            await _timeBlockRepository.UpdateTimeBlockAsync(block);
        }

        public async Task<IEnumerable<TimeBlock>> GenerateTimeBlocksForTasksAsync(IEnumerable<TaskItem> tasks)
        {
            var timeBlocks = new List<TimeBlock>();
            DateTime currentTime = DateTime.Today.AddHours(9); // Начало дня, например, с 9:00 утра

            // Сортировка задач в порядке убывания важности
            var sortedTasks = tasks.OrderBy(t => t.Category).ThenByDescending(t => t.Priority);

            foreach (var task in sortedTasks)
            {
                // Создание временного блока для задачи
                var timeBlock = new TimeBlock
                {
                    StartTime = currentTime,
                    EndTime = currentTime.Add(task.EstimatedDuration),
                    Title = task.Title,
                    Description = task.Description,
                    TaskItemId = task.Id,
                    BlockType = TimeBlockType.Work
                };

                timeBlocks.Add(timeBlock);
                currentTime = timeBlock.EndTime;

                // Добавление перерывов после каждых 2 часов работы
                if (timeBlock.EndTime.Subtract(timeBlock.StartTime).TotalHours >= 2)
                {
                    var breakBlock = new TimeBlock
                    {
                        StartTime = currentTime,
                        EndTime = currentTime.AddMinutes(30), 
                        Title = "Перерыв",
                        BlockType = TimeBlockType.Break
                    };
                    timeBlocks.Add(breakBlock);
                    currentTime = breakBlock.EndTime;
                }
            }

            // Сохранение временных блоков
            foreach (var block in timeBlocks)
            {
                await AddTimeBlockAsync(block);
            }

            return timeBlocks;
        }

    }
}
