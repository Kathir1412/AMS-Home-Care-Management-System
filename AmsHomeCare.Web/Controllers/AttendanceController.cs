using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AmsHomeCare.Core.Entities;
using AmsHomeCare.Core.Enums;
using AmsHomeCare.Core.Interfaces;

namespace AmsHomeCare.Web.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Attendance
        public async Task<IActionResult> Index(DateTime? month)
        {
            var targetMonth = month ?? DateTime.Today;
            var startOfMonth = new DateTime(targetMonth.Year, targetMonth.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var attendances = await _unitOfWork.Attendances.FindAsync(
                a => a.Date >= startOfMonth && a.Date <= endOfMonth,
                a => a.Employee!
            );

            ViewBag.SelectedMonth = targetMonth.ToString("yyyy-MM");
            return View(attendances.OrderByDescending(a => a.Date).ThenBy(a => a.Employee?.Name));
        }

        // GET: Attendance/DailySheet
        public async Task<IActionResult> DailySheet(DateTime? date)
        {
            var targetDate = date ?? DateTime.Today;
            ViewBag.TargetDate = targetDate.ToString("yyyy-MM-dd");

            var employees = await _unitOfWork.Employees.FindAsync(e => e.Status == EmployeeStatus.Active);
            var existingAttendances = await _unitOfWork.Attendances.FindAsync(a => a.Date.Date == targetDate.Date);

            var list = new List<AttendanceSheetViewModel>();
            foreach (var emp in employees)
            {
                var existing = existingAttendances.FirstOrDefault(a => a.EmployeeId == emp.Id);
                list.Add(new AttendanceSheetViewModel
                {
                    EmployeeId = emp.Id,
                    EmployeeCode = emp.EmployeeCode,
                    EmployeeName = emp.Name,
                    Role = emp.Role.ToString(),
                    InTime = existing?.InTime,
                    OutTime = existing?.OutTime,
                    Status = existing?.Status ?? AttendanceStatus.Present,
                    OvertimeHours = existing?.OvertimeHours ?? 0
                });
            }

            return View(list);
        }

        // POST: Attendance/SaveDailySheet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDailySheet(DateTime date, List<AttendanceSheetViewModel> models)
        {
            if (models != null && models.Any())
            {
                var existingAttendances = await _unitOfWork.Attendances.FindAsync(a => a.Date.Date == date.Date);

                foreach (var model in models)
                {
                    var existing = existingAttendances.FirstOrDefault(a => a.EmployeeId == model.EmployeeId);
                    if (existing != null)
                    {
                        existing.InTime = model.InTime;
                        existing.OutTime = model.OutTime;
                        existing.Status = model.Status;
                        existing.OvertimeHours = model.OvertimeHours;
                        _unitOfWork.Attendances.Update(existing);
                    }
                    else
                    {
                        var newAttendance = new Attendance
                        {
                            EmployeeId = model.EmployeeId,
                            Date = date,
                            InTime = model.InTime,
                            OutTime = model.OutTime,
                            Status = model.Status,
                            OvertimeHours = model.OvertimeHours
                        };
                        await _unitOfWork.Attendances.AddAsync(newAttendance);
                    }
                }

                await _unitOfWork.CompleteAsync();
                TempData["Success"] = $"Attendance successfully updated for date: {date:dd-MMM-yyyy}";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(DailySheet), new { date = date.ToString("yyyy-MM-dd") });
        }
    }

    public class AttendanceSheetViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public TimeSpan? InTime { get; set; }
        public TimeSpan? OutTime { get; set; }
        public AttendanceStatus Status { get; set; }
        public double OvertimeHours { get; set; }
    }
}
