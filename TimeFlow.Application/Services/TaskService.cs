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
            return await _taskRepository.GetTasksAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByCategoryAsync(TaskCategory category)
        {
            return await _taskRepository.GetTasksAsync(task => task.Category == category);
        }

        public async Task<TaskItem> GetTaskByIdAsync(int id)
        {
            return await _taskRepository.GetTaskByIdAsync(id);
        }

        public async Task AddTaskAsync(TaskItem task)
        {
            await _taskRepository.AddTaskAsync(task);
        }

        public async Task UpdateTaskAsync(TaskItem task)
        {
            await _taskRepository.UpdateTaskAsync(task);
        }

        public async Task DeleteTaskAsync(int id)
        {
            await _taskRepository.DeleteTaskAsync(id);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByDateAsync(DateTime date)
        {
            return await _taskRepository.GetTasksAsync(task => task.ScheduledDate.Date == date.Date);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _taskRepository.GetTasksAsync(task => task.ScheduledDate >= startDate && task.ScheduledDate <= endDate);
        }
    }
}
