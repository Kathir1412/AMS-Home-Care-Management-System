using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AmsHomeCare.Core.Entities;
using AmsHomeCare.Core.Interfaces;
using AmsHomeCare.Infrastructure.Identity;

namespace AmsHomeCare.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public SettingsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // GET: Settings
        public async Task<IActionResult> Index()
        {
            var settings = await _unitOfWork.Settings.GetAllAsync();
            var holidays = await _unitOfWork.Holidays.GetAllAsync();
            var users = _userManager.Users.ToList();

            ViewBag.CompanyName = settings.FirstOrDefault(s => s.Key == "CompanyName")?.Value ?? "";
            ViewBag.CompanyAddress = settings.FirstOrDefault(s => s.Key == "CompanyAddress")?.Value ?? "";
            ViewBag.CompanyPhone = settings.FirstOrDefault(s => s.Key == "CompanyPhone")?.Value ?? "";
            ViewBag.CompanyEmail = settings.FirstOrDefault(s => s.Key == "CompanyEmail")?.Value ?? "";

            ViewBag.Holidays = holidays.OrderBy(h => h.Date);
            ViewBag.Users = users;

            return View(new Holiday { Date = DateTime.Today });
        }

        // POST: Settings/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string companyName, string companyAddress, string companyPhone, string companyEmail)
        {
            var settings = await _unitOfWork.Settings.GetAllAsync();
            
            void SaveSetting(string key, string val)
            {
                var existing = settings.FirstOrDefault(s => s.Key == key);
                if (existing != null)
                {
                    existing.Value = val;
                    _unitOfWork.Settings.Update(existing);
                }
                else
                {
                    _unitOfWork.Settings.AddAsync(new Setting { Key = key, Value = val }).Wait();
                }
            }

            SaveSetting("CompanyName", companyName);
            SaveSetting("CompanyAddress", companyAddress);
            SaveSetting("CompanyPhone", companyPhone);
            SaveSetting("CompanyEmail", companyEmail);

            await _unitOfWork.CompleteAsync();
            TempData["Success"] = "Company Profile updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Settings/CreateHoliday
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHoliday(Holiday holiday)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Holidays.AddAsync(holiday);
                await _unitOfWork.CompleteAsync();
                TempData["Success"] = "Holiday added successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Settings/DeleteHoliday/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHoliday(int id)
        {
            var holiday = await _unitOfWork.Holidays.GetByIdAsync(id);
            if (holiday != null)
            {
                _unitOfWork.Holidays.Remove(holiday);
                await _unitOfWork.CompleteAsync();
                TempData["Success"] = "Holiday deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Settings/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string userId, string newPassword)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newPassword))
            {
                TempData["Error"] = "User ID and New Password are required.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User account not found.";
                return RedirectToAction(nameof(Index));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = $"Password successfully updated for user: {user.Email}";
            }
            else
            {
                TempData["Error"] = "Failed to update password: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
