namespace TimeFlow.Domain.Entities
{
    public class TimeBlock
    {
        public int Id { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        // Идентификатор связанной задачи (если есть)
        public int? TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; }

        public TimeBlockType BlockType { get; set; }

        public string Notes { get; set; }
    }

    public enum TimeBlockType
    {
        Work,
        Break,
        Exercise,
        Personal,
        Other
    }
}
