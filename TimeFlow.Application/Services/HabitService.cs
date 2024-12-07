using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Core.Services
{
    public class HabitService : IHabitService
    {
        private readonly IHabitRepository _habitRepository;

        public HabitService(IHabitRepository habitRepository)
        {
            _habitRepository = habitRepository;
        }

        public async Task<Habit> GetHabitByIdAsync(int id)
        {
            var habit = await _habitRepository.GetHabitWithDetailsAsync(id);
            if (habit == null)
                throw new KeyNotFoundException($"Привычка с id {id} не найдена.");
            return habit;
        }

        public async Task<IEnumerable<Habit>> GetAllHabitsAsync()
        {
            return await _habitRepository.GetAllHabitsWithDetailsAsync();
        }

        public async Task CreateHabitAsync(Habit habit)
        {
            if (habit == null)
                throw new ArgumentNullException(nameof(habit));

            await _habitRepository.AddAsync(habit);
            await _habitRepository.SaveChangesAsync();
        }

        public async Task UpdateHabitAsync(Habit habit)
        {
            if (habit == null)
                throw new ArgumentNullException(nameof(habit));

            var existingHabit = await _habitRepository.GetHabitWithDetailsAsync(habit.Id);
            if (existingHabit == null)
                throw new KeyNotFoundException($"Привычка с id {habit.Id} не найдена.");

            _habitRepository.Update(existingHabit);
            await _habitRepository.SaveChangesAsync();
        }

        public async Task DeleteHabitAsync(int id)
        {
            var habit = await _habitRepository.GetHabitWithDetailsAsync(id);
            if (habit == null)
                throw new KeyNotFoundException($"Привычка с id  {id}  не найдена.");

            _habitRepository.Remove(habit);
            await _habitRepository.SaveChangesAsync();
        }

        public async Task AddCompletionRecordAsync(int habitId, HabitRecord record)
        {
            var habit = await _habitRepository.GetHabitWithDetailsAsync(habitId);
            if (habit == null)
                throw new KeyNotFoundException($"Привычка с id  {habitId}  не найдена.");

            habit.CompletionRecords.Add(record);
            UpdateStreak(habit, record);

            _habitRepository.Update(habit);
            await _habitRepository.SaveChangesAsync();
        }

        private void UpdateStreak(Habit habit, HabitRecord record)
        {
            if (habit.LastCompletionDate.HasValue)
            {
                var daysDifference = (record.Date.Date - habit.LastCompletionDate.Value.Date).Days;

                if (daysDifference == 1)
                {
                    habit.CurrentStreak += 1;
                }
                else if (daysDifference <= habit.AllowedMissedDays + 1)
                {
                    habit.CurrentStreak += 1;
                }
                else
                {
                    habit.CurrentStreak = 1;
                }
            }
            else
            {
                habit.CurrentStreak = 1;
            }

            if (habit.CurrentStreak > habit.LongestStreak)
            {
                habit.LongestStreak = habit.CurrentStreak;
            }

            habit.LastCompletionDate = record.Date;
        }

        public async Task<IEnumerable<Habit>> GetHabitsForMonth(int year, int month)
        {
            var allHabits = await _habitRepository.GetAllAsync();

            foreach (var habit in allHabits)
            {
                habit.CompletionRecords = habit.CompletionRecords
                    .Where(r => r.Date.Year == year && r.Date.Month == month)
                    .ToList();
            }

            return allHabits;
        }

    }
}
