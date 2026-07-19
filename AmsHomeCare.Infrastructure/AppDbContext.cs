using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AmsHomeCare.Core.Entities;
using AmsHomeCare.Infrastructure.Identity;

namespace AmsHomeCare.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<DutyAssignment> DutyAssignments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Holiday> Holidays { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Employee
            builder.Entity<Employee>()
                .HasIndex(e => e.EmployeeCode)
                .IsUnique();

            builder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            // Configure Patient relationships
            builder.Entity<Patient>()
                .HasOne(p => p.AssignedEmployee)
                .WithMany()
                .HasForeignKey(p => p.AssignedEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure DutyAssignment relationships
            builder.Entity<DutyAssignment>()
                .HasOne(da => da.Employee)
                .WithMany()
                .HasForeignKey(da => da.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DutyAssignment>()
                .HasOne(da => da.Patient)
                .WithMany()
                .HasForeignKey(da => da.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DutyAssignment>()
                .HasOne(da => da.Shift)
                .WithMany()
                .HasForeignKey(da => da.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Attendance relationships
            builder.Entity<Attendance>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Leave relationships
            builder.Entity<Leave>()
                .HasOne(l => l.Employee)
                .WithMany()
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
