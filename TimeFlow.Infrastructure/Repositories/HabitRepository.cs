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

        protected readonly DbSet<HabitRecord> _records;
        public HabitRepository(AppDbContext context) : base(context) {
            _records = context.Set<HabitRecord>();
        }


        public async Task<Habit> GetHabitWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(h => h.Stages)
                .Include(h => h.Periodicity)
                .Include(h => h.CompletionRecords)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<IEnumerable<Habit>> GetAllHabitsWithDetailsAsync()
        {
            return await _dbSet
                .Include(h => h.Stages)
                .Include(h => h.Periodicity)
                .Include(h => h.CompletionRecords)
                .ToListAsync();
        }

        public async Task AddHabitRecord(HabitRecord habitRecord)
        {
            await _records.AddAsync(habitRecord);
        }
    }
}
