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
    public class PatientController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Patient
        public async Task<IActionResult> Index()
        {
            var patients = await _unitOfWork.Patients.GetAllAsync(p => p.AssignedEmployee!);
            return View(patients);
        }

        // GET: Patient/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _unitOfWork.Patients.FindAsync(p => p.Id == id, p => p.AssignedEmployee!);
            var singlePatient = patient.FirstOrDefault();
            if (singlePatient == null)
            {
                return NotFound();
            }
            return View(singlePatient);
        }

        // GET: Patient/Create
        public async Task<IActionResult> Create()
        {
            var employees = await _unitOfWork.Employees.FindAsync(e => e.Status == Core.Enums.EmployeeStatus.Active);
            ViewBag.Employees = new SelectList(employees, "Id", "Name");
            return View(new Patient { StartDate = DateTime.Today });
        }

        // POST: Patient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            ModelState.Remove("AssignedEmployee");
            if (ModelState.IsValid)
            {
                await _unitOfWork.Patients.AddAsync(patient);
                await _unitOfWork.CompleteAsync();
                TempData["Success"] = "Patient registered successfully.";
                return RedirectToAction(nameof(Index));
            }

            var employees = await _unitOfWork.Employees.FindAsync(e => e.Status == Core.Enums.EmployeeStatus.Active);
            ViewBag.Employees = new SelectList(employees, "Id", "Name", patient.AssignedEmployeeId);
            return View(patient);
        }

        // GET: Patient/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            var employees = await _unitOfWork.Employees.FindAsync(e => e.Status == Core.Enums.EmployeeStatus.Active);
            ViewBag.Employees = new SelectList(employees, "Id", "Name", patient.AssignedEmployeeId);
            return View(patient);
        }

        // POST: Patient/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient patient)
        {
            if (id != patient.Id)
            {
                return NotFound();
            }

            ModelState.Remove("AssignedEmployee");
            if (ModelState.IsValid)
            {
                _unitOfWork.Patients.Update(patient);
                await _unitOfWork.CompleteAsync();
                TempData["Success"] = "Patient details updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            var employees = await _unitOfWork.Employees.FindAsync(e => e.Status == Core.Enums.EmployeeStatus.Active);
            ViewBag.Employees = new SelectList(employees, "Id", "Name", patient.AssignedEmployeeId);
            return View(patient);
        }

        // POST: Patient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            _unitOfWork.Patients.Remove(patient);
            await _unitOfWork.CompleteAsync();
            TempData["Success"] = "Patient record deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
