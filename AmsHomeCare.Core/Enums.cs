namespace AmsHomeCare.Core.Enums
{
    public enum EmployeeRole
    {
        Nurse,
        Attender,
        Physiotherapist,
        Caregiver,
        Driver
    }

    public enum EmployeeStatus
    {
        Active,
        Inactive
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public enum ServiceType
    {
        LongTermCare,
        ShortTermCare,
        Physiotherapy,
        PostSurgicalCare,
        PalliativeCare,
        DailyAssistance,
        Transportation
    }

    public enum ShiftType
    {
        Morning,
        Evening,
        Night,
        Custom
    }

    public enum AttendanceStatus
    {
        Present,
        Absent,
        HalfDay,
        Leave
    }

    public enum LeaveType
    {
        CasualLeave,
        SickLeave,
        EarnedLeave,
        MaternityLeave,
        PaternityLeave,
        LossOfPay
    }

    public enum LeaveStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
