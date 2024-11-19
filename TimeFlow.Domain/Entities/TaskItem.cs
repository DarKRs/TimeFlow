namespace TimeFlow.Domain.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime PlannedStart { get; set; }
        public DateTime PlannedEnd { get; set; }
        public TimeSpan EstimatedDuration { get; set; } = TimeSpan.FromHours(2); // значение по умолчанию — 2 часа (полный цикл Pomodoro)
        public bool IsImportant { get; set; }
        public bool IsUrgent { get; set; }
        public TaskCategory Category
        {
            get
            {
                if (IsImportant && IsUrgent)
                    return TaskCategory.UrgentImportant;
                if (IsImportant && !IsUrgent)
                    return TaskCategory.NotUrgentImportant;
                if (!IsImportant && IsUrgent)
                    return TaskCategory.UrgentNotImportant;
                else
                    return TaskCategory.NotUrgentNotImportant;
            }
        }

        // Связь с PomodoroSession
        public ICollection<PomodoroSession> PomodoroSessions { get; set; }

        // Связь с TimeBlock
        public ICollection<TimeBlock> TimeBlocks { get; set; }
    }

    public enum TaskCategory
    {
        UrgentImportant,
        NotUrgentImportant,
        UrgentNotImportant,
        NotUrgentNotImportant
    }
}
