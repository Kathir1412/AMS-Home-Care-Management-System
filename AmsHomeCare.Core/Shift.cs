using System;

namespace AmsHomeCare.Core.Entities
{
    public class Shift
    {
        public int Id { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DayOfWeek? WeeklyOffDay { get; set; }
        public bool IsCustom { get; set; }
    }
}
