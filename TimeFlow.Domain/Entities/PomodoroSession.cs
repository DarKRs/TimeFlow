namespace TimeFlow.Domain.Entities
{
    //https://ru.wikipedia.org/wiki/%D0%9C%D0%B5%D1%82%D0%BE%D0%B4_%D0%BF%D0%BE%D0%BC%D0%B8%D0%B4%D0%BE%D1%80%D0%B0
    public class PomodoroSession
    {
        public int Id { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        // Количество завершённых интервалов
        public int CompletedIntervals { get; set; }

        // Общее количество интервалов в сессии
        public int TotalIntervals { get; set; }

        public int WorkIntervalDuration { get; set; } // в минутах
        public int ShortBreakDuration { get; set; }   // в минутах
        public int LongBreakDuration { get; set; }    // в минутах

        // Количество интервалов перед длинным перерывом
        public int IntervalsBeforeLongBreak { get; set; }

        public PomodoroSessionStatus Status { get; set; }

        // Конструктор по умолчанию
        public PomodoroSession()
        {
            // Значения по умолчанию
            WorkIntervalDuration = 25;
            ShortBreakDuration = 5;
            LongBreakDuration = 15;
            IntervalsBeforeLongBreak = 4;
            TotalIntervals = 0;
            CompletedIntervals = 0;
            Status = PomodoroSessionStatus.NotStarted;
        }
    }

    public enum PomodoroSessionStatus
    {
        NotStarted,
        InProgress,
        OnBreak,
        Completed,
        Canceled
    }

    public enum PomodoroSessionType
    {
        Work,
        ShortBreak,
        LongBreak
    }
}
