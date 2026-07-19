using System;
using AmsHomeCare.Core.Enums;

namespace AmsHomeCare.Core.Entities
{
    public class Attendance
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? InTime { get; set; }
        public TimeSpan? OutTime { get; set; }
        public AttendanceStatus Status { get; set; }
        public double OvertimeHours { get; set; }
    }
}
