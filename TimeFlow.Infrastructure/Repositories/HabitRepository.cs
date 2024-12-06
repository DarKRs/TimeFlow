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
    public class HabitRepository : BaseRepository<Habit>, IHabitRepository
    {
        public HabitRepository(AppDbContext context) : base(context) { }

        public async Task<Habit> GetHabitWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(h => h.Stages)
                .Include(h => h.Periodicity)
                    .ThenInclude(p => p.DaysOfWeek)
                .Include(h => h.CompletionRecords)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<IEnumerable<Habit>> GetAllHabitsWithDetailsAsync()
        {
            return await _dbSet
                .Include(h => h.Stages)
                .Include(h => h.Periodicity)
                    .ThenInclude(p => p.DaysOfWeek)
                .Include(h => h.CompletionRecords)
                .ToListAsync();
        }

    }
}
