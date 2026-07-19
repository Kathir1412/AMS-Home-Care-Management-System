using System;
using AmsHomeCare.Core.Enums;

namespace AmsHomeCare.Core.Entities
{
    public class Leave
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveType LeaveType { get; set; }
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public string Remarks { get; set; } = string.Empty;
    }
}
