using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AmsHomeCare.Core.Entities;
using AmsHomeCare.Core.Enums;
using AmsHomeCare.Core.Interfaces;
using AmsHomeCare.Web.Models;

namespace AmsHomeCare.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;

            var employees = await _unitOfWork.Employees.GetAllAsync();
            var patients = await _unitOfWork.Patients.GetAllAsync();
            
            var todayAttendances = await _unitOfWork.Attendances.FindAsync(
                a => a.Date.Date == today
            );
            
            var presentToday = todayAttendances.Count(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.HalfDay);
            
            var onLeaveToday = todayAttendances.Count(a => a.Status == AttendanceStatus.Leave);

            // Fetch today's duty schedule
            var todaySchedule = await _unitOfWork.DutyAssignments.FindAsync(
                da => da.StartDate.Date <= today && da.EndDate.Date >= today,
                da => da.Employee!,
                da => da.Patient!,
                da => da.Shift!
            );

            // Fetch recent activities dynamically
            var recentActivities = new List<string>();

            var recentAttendances = (await _unitOfWork.Attendances.GetAllAsync(a => a.Employee!))
                .OrderByDescending(a => a.Id)
                .Take(3);

            foreach (var att in recentAttendances)
            {
                var timeStr = att.InTime.HasValue ? $" at {att.InTime.Value:hh\\:mm}" : "";
                recentActivities.Add($"Attendance: {att.Employee?.Name} marked {att.Status}{timeStr} on {att.Date:dd-MMM-yyyy}.");
            }

            var recentLeaves = (await _unitOfWork.Leaves.GetAllAsync(l => l.Employee!))
                .OrderByDescending(l => l.Id)
                .Take(3);

            foreach (var leave in recentLeaves)
            {
                recentActivities.Add($"Leave: {leave.Employee?.Name} applied for {leave.LeaveType} ({leave.Status}) from {leave.StartDate:dd-MMM} to {leave.EndDate:dd-MMM}.");
            }

            if (!recentActivities.Any())
            {
                recentActivities.Add("No recent activity found. System is up to date.");
            }

            var model = new DashboardViewModel
            {
                TotalEmployees = employees.Count(),
                TotalPatients = patients.Count(),
                PresentToday = presentToday,
                OnLeaveToday = onLeaveToday,
                TodaySchedule = todaySchedule,
                RecentActivities = recentActivities
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
