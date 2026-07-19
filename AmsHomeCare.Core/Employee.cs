using System;
using AmsHomeCare.Core.Enums;

namespace AmsHomeCare.Core.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? PhotoPath { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string MobileNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string AadhaarNumber { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public double Experience { get; set; } // in years
        public DateTime JoiningDate { get; set; }
        public EmployeeRole Role { get; set; }
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
    }
}
