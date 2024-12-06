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
