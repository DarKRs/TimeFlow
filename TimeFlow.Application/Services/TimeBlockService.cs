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
            DateTime currentTime = DateTime.Today.AddHours(9); // Начало дня

            // Сортировка задач в порядке убывания важности
            foreach (var task in tasks.OrderBy(t => t.Category).ThenBy(t => t.ScheduledDate).ThenByDescending(t => t.EstimatedDuration))
            {
                var block = CreateTimeBlock(task, currentTime);
                timeBlocks.Add(block);
                currentTime = block.EndTime;

                // Добавление перерывов после каждых 2 часов работы
                if (block.EndTime.Subtract(block.StartTime).TotalHours >= 2)
                {
                    var breakBlock = CreateBreakBlock(currentTime);
                    timeBlocks.Add(breakBlock);
                    currentTime = breakBlock.EndTime;
                }
            }

            foreach (var block in timeBlocks)
            {
                await AddTimeBlockAsync(block);
            }
            return timeBlocks;
        }

        private TimeBlock CreateTimeBlock(TaskItem task, DateTime startTime)
        {
            return new TimeBlock
            {
                StartTime = startTime,
                EndTime = startTime.Add(task.EstimatedDuration),
                Title = task.Title,
                Description = task.Description,
                TaskItemId = task.Id,
                BlockType = TimeBlockType.Work
            };
        }

        private TimeBlock CreateBreakBlock(DateTime startTime)
        {
            return new TimeBlock
            {
                StartTime = startTime,
                EndTime = startTime.AddMinutes(30),
                Title = "Перерыв",
                BlockType = TimeBlockType.Break
            };
        }


    }
}
