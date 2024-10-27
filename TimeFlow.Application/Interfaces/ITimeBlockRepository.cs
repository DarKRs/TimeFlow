using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Core.Interfaces
{
    public interface ITimeBlockRepository
    {
        Task<IEnumerable<TimeBlock>> GetTimeBlocksByDateAsync(DateTime date);
        Task AddTimeBlockAsync(TimeBlock block);
        Task UpdateTimeBlockAsync(TimeBlock block);
        Task DeleteTimeBlockAsync(int id);
    }
}
