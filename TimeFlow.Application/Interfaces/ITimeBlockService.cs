using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Core.Interfaces
{
    public interface ITimeBlockService
    {
        Task<IEnumerable<TimeBlock>> GetTimeBlocksForTodayAsync();
        Task<IEnumerable<TimeBlock>> GetTimeBlocksByDateAsync(DateTime date);
        Task<IEnumerable<TimeBlock>> GenerateTimeBlocksForTasksAsync(IEnumerable<TaskItem> tasks);
        Task AddTimeBlockAsync(TimeBlock block);
        Task UpdateTimeBlockAsync(TimeBlock block);
        Task DeleteTimeBlockAsync(int id);
    }
}
