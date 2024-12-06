using System.Linq.Expressions;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Core.Interfaces
{
    public interface ITaskRepository : IBaseRepository<TaskItem>
    {
        //Для специфичного функционала
        Task<TaskItem> GetTaskWithPomodoroAsync(int id);
    }
}
