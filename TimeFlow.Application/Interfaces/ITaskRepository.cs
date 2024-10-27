﻿using TimeFlow.Domain.Entities;

namespace TimeFlow.Core.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<IEnumerable<TaskItem>> GetTasksByCategoryAsync(TaskCategory category);
        Task<TaskItem> GetTaskByIdAsync(int id);
        Task AddTaskAsync(TaskItem task);
        Task UpdateTaskAsync(TaskItem task);
        Task DeleteTaskAsync(int id);
    }
}
