using Microsoft.EntityFrameworkCore;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;
using TimeFlow.Infrastructure.Data;

namespace TimeFlow.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _context.Tasks.ToListAsync();
        }
        public async Task<IEnumerable<TaskItem>> GetTasksByCategoryAsync(TaskCategory category)
        {
            return await _context.Tasks
                .Where(t => t.Category == category)
                .ToListAsync();
        }

        public async Task<TaskItem> GetTaskByIdAsync(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task AddTaskAsync(TaskItem task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByDateAsync(DateTime date)
        {
            return await _context.Tasks
                .Where(tb => tb.ScheduledDate.Date == date)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Tasks
                .Where(t => t.ScheduledDate >= startDate && t.ScheduledDate < endDate)
                .ToListAsync();
        }
    }
}
