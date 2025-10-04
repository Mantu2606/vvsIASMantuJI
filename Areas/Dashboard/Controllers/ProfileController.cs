using FossTech.Areas.Dashboard.ViewModels;
using FossTech.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FossTech.Models;
using System.IO;
using System.Threading.Tasks;
using FossTech.Areas.FossTech.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace FossTech.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize(Roles ="User")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var model = new Dashboard.ViewModels.UpdateStudentProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfilePhoto,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Dashboard.ViewModels.UpdateStudentProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                if (model.ProfilePhoto != null)
                {
                    if (!string.IsNullOrEmpty(user.ProfilePhoto))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfilePhoto.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    var fileName = Path.GetFileName(model.ProfilePhoto.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "ProfileImage", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfilePhoto.CopyToAsync(stream);
                    }

                    user.ProfilePhoto = "/img/ProfileImage/" + fileName;
                }
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    if (model.NewPassword != model.ConfirmNewPassword)
                    {
                        ModelState.AddModelError(string.Empty, "The new password and confirmation do not match.");
                        return View(model);
                    }
                    if (model.NewPassword == model.CurrentPassword)
                    {
                        ModelState.AddModelError(string.Empty, "New password cannot be the same as the current password.");
                        return View(model);
                    }
                    var passwordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                    if (!passwordValid)
                    {
                        ModelState.AddModelError(string.Empty, "The current password is incorrect.");
                        return View(model);
                    }
                    var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (!changePasswordResult.Succeeded)
                    {
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);  
                    }
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Your profile has been updated successfully!";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}
