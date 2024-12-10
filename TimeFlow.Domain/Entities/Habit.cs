using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeFlow.Domain.Entities
{
    public class Habit
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<HabitStage> Stages { get; set; }

        // Периодичность выполнения
        public HabitPeriodicity Periodicity { get; set; }

        // Записи выполнения
        public ICollection<HabitRecord> CompletionRecords { get; set; }

        // Цепочка выполнения
        public int CurrentStreak { get; set; } = 0;
        public int LongestStreak { get; set; } = 0;
        public DateTime? LastCompletionDate { get; set; }

        public int AllowedMissedDays { get; set; } = 1;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public bool IsCompletedToday => CompletionRecords.Any(r => r.Date.Date == DateTime.UtcNow.Date && r.Status == CompletionStatus.Done);

        public double GetCompletionPercentage(DateTime startDate, DateTime endDate)
        {
            var records = CompletionRecords.Where(r => r.Date.Date >= startDate.Date && r.Date.Date <= endDate.Date).ToList();
            if (records.Count == 0) return 0;

            double countDone = records.Count(r => r.Status == CompletionStatus.Done);
            return countDone / records.Count;
        }

        public int GetLongestStreak(DateTime startDate, DateTime endDate, int allowedMissedDays = 1)
        {
            // Простой расчёт серии: итерация по дням, проверка выполенения
            var days = Enumerable.Range(0, (endDate - startDate).Days + 1).Select(d => startDate.AddDays(d));
            int currentStreak = 0;
            int longestStreak = 0;
            int missedDaysInARow = 0;

            foreach (var day in days)
            {
                var record = CompletionRecords.FirstOrDefault(r => r.Date.Date == day.Date);
                bool done = record != null && record.Status == CompletionStatus.Done;

                if (done)
                {
                    currentStreak++;
                    if (currentStreak > longestStreak) longestStreak = currentStreak;
                    missedDaysInARow = 0;
                }
                else
                {
                    missedDaysInARow++;
                    if (missedDaysInARow > allowedMissedDays)
                    {
                        currentStreak = 0;
                        missedDaysInARow = 0;
                    }
                }
            }

            return longestStreak;
        }
    }

    public class HabitStage
    {
        public int Id { get; set; }
        public int HabitId { get; set; }
        public Habit Habit { get; set; }

        public string Description { get; set; }
        public CompletionStatus Status { get; set; }
        public int Order { get; set; }
    }

    public class HabitPeriodicity
    {
        public int Id { get; set; }
        public int HabitId { get; set; }
        public Habit Habit { get; set; }
        public Frequency FrequencyType { get; set; }
        public string DaysOfWeek { get; set; }
    }

    public class HabitRecord
    {
        public int Id { get; set; }
        public int HabitId { get; set; }
        public Habit Habit { get; set; }

        public DateTime Date { get; set; }
        public CompletionStatus Status { get; set; }
    }

    public enum Frequency
    {
        Daily,
        Weekly,
        Custom
    }

    public enum CompletionStatus
    {
        NotDone,
        PartiallyDone,
        Done
    }
}
