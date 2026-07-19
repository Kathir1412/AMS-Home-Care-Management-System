using System;
using System.Threading.Tasks;
using AmsHomeCare.Core.Entities;
using AmsHomeCare.Core.Interfaces;
using AmsHomeCare.Infrastructure.Data;
using AmsHomeCare.Infrastructure.Repositories;

namespace AmsHomeCare.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Employees = new Repository<Employee>(_context);
            Patients = new Repository<Patient>(_context);
            Shifts = new Repository<Shift>(_context);
            DutyAssignments = new Repository<DutyAssignment>(_context);
            Attendances = new Repository<Attendance>(_context);
            Leaves = new Repository<Leave>(_context);
            Settings = new Repository<Setting>(_context);
            Holidays = new Repository<Holiday>(_context);
        }

        public IRepository<Employee> Employees { get; }
        public IRepository<Patient> Patients { get; }
        public IRepository<Shift> Shifts { get; }
        public IRepository<DutyAssignment> DutyAssignments { get; }
        public IRepository<Attendance> Attendances { get; }
        public IRepository<Leave> Leaves { get; }
        public IRepository<Setting> Settings { get; }
        public IRepository<Holiday> Holidays { get; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
