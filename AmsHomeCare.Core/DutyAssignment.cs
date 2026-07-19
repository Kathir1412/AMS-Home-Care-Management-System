using System;

namespace AmsHomeCare.Core.Entities
{
    public class DutyAssignment
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public int ShiftId { get; set; }
        public Shift? Shift { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SupervisorNotes { get; set; } = string.Empty;
    }
}
