using System;
using AmsHomeCare.Core.Enums;

namespace AmsHomeCare.Core.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public string Address { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string EmergencyContact { get; set; } = string.Empty;
        public string MedicalCondition { get; set; } = string.Empty;
        public ServiceType ServiceType { get; set; }
        public int? AssignedEmployeeId { get; set; }
        public Employee? AssignedEmployee { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
