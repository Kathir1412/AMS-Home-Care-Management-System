using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AmsHomeCare.Core.Entities;
using AmsHomeCare.Core.Interfaces;

namespace AmsHomeCare.Web.Controllers
{
    [Authorize]
    public class ShiftController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShiftController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Shift
        public async Task<IActionResult> Index()
        {
            var shifts = await _unitOfWork.Shifts.GetAllAsync();
            return View(shifts);
        }

        // GET: Shift/Create
        public IActionResult Create()
        {
            return View(new Shift { StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0) });
        }

        // POST: Shift/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Shift shift)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Shifts.AddAsync(shift);
                await _unitOfWork.CompleteAsync();
                TempData["Success"] = "Shift configuration created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(shift);
        }

        // GET: Shift/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var shift = await _unitOfWork.Shifts.GetByIdAsync(id);
            if (shift == null)
            {
                return NotFound();
            }
            return View(shift);
        }

        // POST: Shift/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Shift shift)
        {
            if (id != shift.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Shifts.Update(shift);
                await _unitOfWork.CompleteAsync();
                TempData["Success"] = "Shift configuration updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(shift);
        }

        // POST: Shift/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shift = await _unitOfWork.Shifts.GetByIdAsync(id);
            if (shift == null)
            {
                return NotFound();
            }

            _unitOfWork.Shifts.Remove(shift);
            await _unitOfWork.CompleteAsync();
            TempData["Success"] = "Shift configuration deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
