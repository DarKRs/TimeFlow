using TimeFlow.Domain.Entities;

namespace TimeFlow.Core.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<IEnumerable<TaskItem>> GetTasksByCategoryAsync(TaskCategory category);
        Task<IEnumerable<TaskItem>> GetTasksByDateAsync(DateTime date);
        Task<IEnumerable<TaskItem>> GetTasksByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<TaskItem> GetTaskByIdAsync(int id);
        Task AddTaskAsync(TaskItem task);
        Task UpdateTaskAsync(TaskItem task);
        Task DeleteTaskAsync(int id);
    }
}
