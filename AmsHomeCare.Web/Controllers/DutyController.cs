using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AmsHomeCare.Core.Entities;
using AmsHomeCare.Core.Interfaces;

namespace AmsHomeCare.Web.Controllers
{
    [Authorize]
    public class DutyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DutyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Duty
        public async Task<IActionResult> Index()
        {
            var assignments = await _unitOfWork.DutyAssignments.GetAllAsync(
                da => da.Employee!,
                da => da.Patient!,
                da => da.Shift!
            );
            return View(assignments);
        }

        // GET: Duty/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View(new DutyAssignment { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(7) });
        }

        // POST: Duty/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DutyAssignment assignment)
        {
            ModelState.Remove("Employee");
            ModelState.Remove("Patient");
            ModelState.Remove("Shift");
            if (ModelState.IsValid)
            {
                await _unitOfWork.DutyAssignments.AddAsync(assignment);
                await _unitOfWork.CompleteAsync();
                TempData["Success"] = "Duty successfully assigned to Employee.";
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdownsAsync(assignment);
            return View(assignment);
        }

        // GET: Duty/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var assignment = await _unitOfWork.DutyAssignments.GetByIdAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            await PopulateDropdownsAsync(assignment);
            return View(assignment);
        }

        // POST: Duty/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DutyAssignment assignment)
        {
            if (id != assignment.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Employee");
            ModelState.Remove("Patient");
            ModelState.Remove("Shift");
            if (ModelState.IsValid)
            {
                _unitOfWork.DutyAssignments.Update(assignment);
                await _unitOfWork.CompleteAsync();
                TempData["Success"] = "Duty assignment updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdownsAsync(assignment);
            return View(assignment);
        }

        // POST: Duty/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignment = await _unitOfWork.DutyAssignments.GetByIdAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            _unitOfWork.DutyAssignments.Remove(assignment);
            await _unitOfWork.CompleteAsync();
            TempData["Success"] = "Duty assignment cancelled successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdownsAsync(DutyAssignment? selected = null)
        {
            var employees = await _unitOfWork.Employees.FindAsync(e => e.Status == Core.Enums.EmployeeStatus.Active);
            var patients = await _unitOfWork.Patients.GetAllAsync();
            var shifts = await _unitOfWork.Shifts.GetAllAsync();

            ViewBag.Employees = new SelectList(employees, "Id", "Name", selected?.EmployeeId);
            ViewBag.Patients = new SelectList(patients, "Id", "PatientName", selected?.PatientId);
            ViewBag.Shifts = new SelectList(shifts, "Id", "ShiftName", selected?.ShiftId);
        }
    }
}
