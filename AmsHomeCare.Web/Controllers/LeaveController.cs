using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AmsHomeCare.Core.Entities;
using AmsHomeCare.Core.Enums;
using AmsHomeCare.Core.Interfaces;

namespace AmsHomeCare.Web.Controllers
{
    [Authorize]
    public class LeaveController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LeaveController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Leave
        public async Task<IActionResult> Index()
        {
            var leaves = await _unitOfWork.Leaves.GetAllAsync(l => l.Employee!);
            return View(leaves.OrderByDescending(l => l.StartDate));
        }

        // GET: Leave/Create
        public async Task<IActionResult> Create()
        {
            var employees = await _unitOfWork.Employees.FindAsync(e => e.Status == EmployeeStatus.Active);
            ViewBag.Employees = new SelectList(employees, "Id", "Name");
            return View(new Leave { StartDate = DateTime.Today, EndDate = DateTime.Today });
        }

        // POST: Leave/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Leave leave)
        {
            ModelState.Remove("Employee");
            if (ModelState.IsValid)
            {
                leave.Status = LeaveStatus.Pending;
                await _unitOfWork.Leaves.AddAsync(leave);
                await _unitOfWork.CompleteAsync();
                TempData["Success"] = "Leave request applied successfully.";
                return RedirectToAction(nameof(Index));
            }

            var employees = await _unitOfWork.Employees.FindAsync(e => e.Status == EmployeeStatus.Active);
            ViewBag.Employees = new SelectList(employees, "Id", "Name", leave.EmployeeId);
            return View(leave);
        }

        // POST: Leave/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var leave = await _unitOfWork.Leaves.GetByIdAsync(id);
            if (leave == null)
            {
                return NotFound();
            }

            leave.Status = LeaveStatus.Approved;
            _unitOfWork.Leaves.Update(leave);

            // Also automatically mark attendance as 'Leave' for those dates!
            for (var date = leave.StartDate.Date; date <= leave.EndDate.Date; date = date.AddDays(1))
            {
                var existingAttendance = (await _unitOfWork.Attendances.FindAsync(
                    a => a.EmployeeId == leave.EmployeeId && a.Date.Date == date
                )).FirstOrDefault();

                if (existingAttendance != null)
                {
                    existingAttendance.Status = AttendanceStatus.Leave;
                    existingAttendance.InTime = null;
                    existingAttendance.OutTime = null;
                    existingAttendance.OvertimeHours = 0;
                    _unitOfWork.Attendances.Update(existingAttendance);
                }
                else
                {
                    var att = new Attendance
                    {
                        EmployeeId = leave.EmployeeId,
                        Date = date,
                        Status = AttendanceStatus.Leave
                    };
                    await _unitOfWork.Attendances.AddAsync(att);
                }
            }

            await _unitOfWork.CompleteAsync();
            TempData["Success"] = "Leave request approved and attendance marked accordingly.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Leave/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var leave = await _unitOfWork.Leaves.GetByIdAsync(id);
            if (leave == null)
            {
                return NotFound();
            }

            leave.Status = LeaveStatus.Rejected;
            _unitOfWork.Leaves.Update(leave);
            await _unitOfWork.CompleteAsync();
            TempData["Success"] = "Leave request rejected.";
            return RedirectToAction(nameof(Index));
        }
    }
}
