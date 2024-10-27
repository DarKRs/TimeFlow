﻿namespace TimeFlow.Domain.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsImportant { get; set; }
        public bool IsUrgent { get; set; }
        public TaskPriority Priority { get; set; }
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

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum TaskCategory
    {
        UrgentImportant,
        NotUrgentImportant,
        UrgentNotImportant,
        NotUrgentNotImportant
    }
}
