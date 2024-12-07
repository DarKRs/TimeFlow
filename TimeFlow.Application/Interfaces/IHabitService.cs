using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Core.Interfaces
{
    public interface IHabitService
    {
        Task<Habit> GetHabitByIdAsync(int id);
        Task<IEnumerable<Habit>> GetAllHabitsAsync();
        Task CreateHabitAsync(Habit habit);
        Task UpdateHabitAsync(Habit habit);
        Task DeleteHabitAsync(int id);

        Task AddCompletionRecordAsync(int habitId, HabitRecord record);

        Task<IEnumerable<Habit>> GetHabitsForMonth(int year, int month);
    }
}
