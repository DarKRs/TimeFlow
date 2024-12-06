using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Core.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByCategoryAsync(TaskCategory category)
        {
            return await _taskRepository.FindAsync(task => task.Category == category);
        }

        public async Task<TaskItem> GetTaskByIdAsync(int id)
        {
            return await _taskRepository.GetByIdAsync(id);
        }

        public async Task AddTaskAsync(TaskItem task)
        {
            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(TaskItem task)
        {
            _taskRepository.Update(task);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id)
        {
            var taskitem = await _taskRepository.GetByIdAsync(id);
            if (taskitem == null)
                throw new KeyNotFoundException($"Не найдена задача с id - {id}");

            _taskRepository.Remove(taskitem);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByDateAsync(DateTime date)
        {
            return await _taskRepository.FindAsync(task => task.ScheduledDate.Date == date.Date);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _taskRepository.FindAsync(task => task.ScheduledDate >= startDate && task.ScheduledDate <= endDate);
        }
    }
}
