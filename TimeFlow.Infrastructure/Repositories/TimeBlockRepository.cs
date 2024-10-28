using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;
using TimeFlow.Infrastructure.Data;

namespace TimeFlow.Infrastructure.Repositories
{
    public class TimeBlockRepository : ITimeBlockRepository
    {
        private readonly AppDbContext _context;

        public TimeBlockRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddTimeBlockAsync(TimeBlock block)
        {
            _context.TimeBlocks.Add(block);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTimeBlockAsync(int id)
        {
            var block = await _context.TimeBlocks.FindAsync(id);
            if (block != null)
            {
                _context.TimeBlocks.Remove(block);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TimeBlock>> GetTimeBlocksByDateAsync(DateTime date)
        {
            return await _context.TimeBlocks
                .Where(tb => tb.StartTime.Date == date)
                .Include(tb => tb.TaskItem)
                .ToListAsync();
        }

        public async Task UpdateTimeBlockAsync(TimeBlock block)
        {
            _context.TimeBlocks.Update(block);
            await _context.SaveChangesAsync();
        }
    }
}
