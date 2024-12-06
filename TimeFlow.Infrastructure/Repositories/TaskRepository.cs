using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;
using TimeFlow.Infrastructure.Data;

namespace TimeFlow.Infrastructure.Repositories
{
    public class TaskRepository : BaseRepository<TaskItem>, ITaskRepository
    {
        public TaskRepository(AppDbContext context) : base(context) { }

        public async Task<TaskItem> GetTaskWithPomodoroAsync(int id)
        {
            return await _dbSet
                .Include(t => t.PomodoroSessions)
                .FirstOrDefaultAsync(h => h.Id == id);
        }
    }
}
