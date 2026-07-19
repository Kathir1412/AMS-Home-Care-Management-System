using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AmsHomeCare.Core.Interfaces;

namespace AmsHomeCare.Web.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Report
        public IActionResult Index()
        {
            return View();
        }

        // GET: Report/DailyAttendance
        public async Task<IActionResult> DailyAttendance(DateTime? date)
        {
            var targetDate = date ?? DateTime.Today;
            var data = await _unitOfWork.Attendances.FindAsync(
                a => a.Date.Date == targetDate.Date,
                a => a.Employee!
            );
            ViewBag.SelectedDate = targetDate.ToString("yyyy-MM-dd");
            return View(data);
        }

        // GET: Report/MonthlyAttendance
        public async Task<IActionResult> MonthlyAttendance(DateTime? month)
        {
            var targetMonth = month ?? DateTime.Today;
            var startOfMonth = new DateTime(targetMonth.Year, targetMonth.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var data = await _unitOfWork.Attendances.FindAsync(
                a => a.Date >= startOfMonth && a.Date <= endOfMonth,
                a => a.Employee!
            );
            ViewBag.SelectedMonth = targetMonth.ToString("yyyy-MM");
            return View(data);
        }

        // GET: Report/EmployeeWise
        public async Task<IActionResult> EmployeeWise(int? employeeId)
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();
            ViewBag.Employees = new SelectList(employees, "Id", "Name", employeeId);

            if (employeeId.HasValue)
            {
                var data = await _unitOfWork.Attendances.FindAsync(
                    a => a.EmployeeId == employeeId.Value,
                    a => a.Employee!
                );
                return View(data.OrderByDescending(a => a.Date));
            }
            return View(Enumerable.Empty<Core.Entities.Attendance>());
        }

        // GET: Report/PatientDuty
        public async Task<IActionResult> PatientDuty(int? patientId)
        {
            var patients = await _unitOfWork.Patients.GetAllAsync();
            ViewBag.Patients = new SelectList(patients, "Id", "PatientName", patientId);

            if (patientId.HasValue)
            {
                var data = await _unitOfWork.DutyAssignments.FindAsync(
                    da => da.PatientId == patientId.Value,
                    da => da.Employee!,
                    da => da.Patient!,
                    da => da.Shift!
                );
                return View(data.OrderByDescending(da => da.StartDate));
            }
            return View(Enumerable.Empty<Core.Entities.DutyAssignment>());
        }
    }
}
