using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeFlow.Domain.Entities
{
    public class TimeBlock
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

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
