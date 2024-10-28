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

    }
}
