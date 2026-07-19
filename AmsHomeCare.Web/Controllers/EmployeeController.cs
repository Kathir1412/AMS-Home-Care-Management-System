using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using AmsHomeCare.Core.Entities;
using AmsHomeCare.Core.Enums;
using AmsHomeCare.Core.Interfaces;
using AmsHomeCare.Web.Models;

namespace AmsHomeCare.Web.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeeController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();
            return View(employees);
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            return View(new EmployeeViewModel { DateOfBirth = DateTime.Today.AddYears(-25), JoiningDate = DateTime.Today });
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Auto generate Employee Code
                var employees = await _unitOfWork.Employees.GetAllAsync();
                var nextNumber = employees.Count() + 1;
                var employeeCode = $"EMP-{nextNumber:D3}";

                string? photoPath = null;
                if (model.PhotoFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PhotoFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.PhotoFile.CopyToAsync(fileStream);
                    }
                    photoPath = "/uploads/" + uniqueFileName;
                }

                var employee = new Employee
                {
                    EmployeeCode = employeeCode,
                    Name = model.Name,
                    PhotoPath = photoPath,
                    Gender = model.Gender,
                    DateOfBirth = model.DateOfBirth,
                    MobileNumber = model.MobileNumber,
                    Email = model.Email,
                    Address = model.Address,
                    AadhaarNumber = model.AadhaarNumber,
                    Qualification = model.Qualification,
                    Experience = model.Experience,
                    JoiningDate = model.JoiningDate,
                    Role = model.Role,
                    Status = model.Status
                };

                await _unitOfWork.Employees.AddAsync(employee);
                await _unitOfWork.CompleteAsync();
                TempData["Success"] = "Employee created successfully with Code: " + employeeCode;
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var model = new EmployeeViewModel
            {
                Id = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                Name = employee.Name,
                PhotoPath = employee.PhotoPath,
                Gender = employee.Gender,
                DateOfBirth = employee.DateOfBirth,
                MobileNumber = employee.MobileNumber,
                Email = employee.Email,
                Address = employee.Address,
                AadhaarNumber = employee.AadhaarNumber,
                Qualification = employee.Qualification,
                Experience = employee.Experience,
                JoiningDate = employee.JoiningDate,
                Role = employee.Role,
                Status = employee.Status
            };

            return View(model);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var employee = await _unitOfWork.Employees.GetByIdAsync(id);
                if (employee == null)
                {
                    return NotFound();
                }

                if (model.PhotoFile != null)
                {
                    // Delete old photo if exists
                    if (!string.IsNullOrEmpty(employee.PhotoPath))
                    {
                        var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, employee.PhotoPath.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PhotoFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.PhotoFile.CopyToAsync(fileStream);
                    }
                    employee.PhotoPath = "/uploads/" + uniqueFileName;
                }

                employee.Name = model.Name;
                employee.Gender = model.Gender;
                employee.DateOfBirth = model.DateOfBirth;
                employee.MobileNumber = model.MobileNumber;
                employee.Email = model.Email;
                employee.Address = model.Address;
                employee.AadhaarNumber = model.AadhaarNumber;
                employee.Qualification = model.Qualification;
                employee.Experience = model.Experience;
                employee.JoiningDate = model.JoiningDate;
                employee.Role = model.Role;
                employee.Status = model.Status;

                _unitOfWork.Employees.Update(employee);
                await _unitOfWork.CompleteAsync();
                TempData["Success"] = "Employee updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            // Optional: Delete photo file
            if (!string.IsNullOrEmpty(employee.PhotoPath))
            {
                var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, employee.PhotoPath.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            _unitOfWork.Employees.Remove(employee);
            await _unitOfWork.CompleteAsync();
            TempData["Success"] = "Employee deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
