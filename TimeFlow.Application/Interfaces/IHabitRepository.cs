using TimeFlow.Domain.Entities;

namespace TimeFlow.Core.Interfaces
{
    public interface IHabitRepository : IBaseRepository<Habit>
    {
        Task<Habit> GetHabitWithDetailsAsync(int id);
        Task<IEnumerable<Habit>> GetAllHabitsWithDetailsAsync();
        Task AddHabitRecord(HabitRecord habitRecord);

    }
}
