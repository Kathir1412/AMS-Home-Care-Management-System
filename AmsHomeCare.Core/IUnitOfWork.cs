using System;
using System.Threading.Tasks;
using AmsHomeCare.Core.Entities;

namespace AmsHomeCare.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Employee> Employees { get; }
        IRepository<Patient> Patients { get; }
        IRepository<Shift> Shifts { get; }
        IRepository<DutyAssignment> DutyAssignments { get; }
        IRepository<Attendance> Attendances { get; }
        IRepository<Leave> Leaves { get; }
        IRepository<Setting> Settings { get; }
        IRepository<Holiday> Holidays { get; }
        Task<int> CompleteAsync();
    }
}
