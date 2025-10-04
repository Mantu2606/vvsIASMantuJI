using DocumentFormat.OpenXml.Spreadsheet;
using FossTech.Areas.FossTech.ViewModels;
using FossTech.Data;
using FossTech.Extensions;
using FossTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class NonVedaStudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NonVedaStudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var nonStudentUsers = _userManager.Users
                .Where(u => u.isStudent == false)  
                .Include(u => u.Branch)            
                .Include(u => u.Board)
                .Include(u => u.Standard)
                .OrderBy(u => u.CreatedAt)
                .ToList();  

            return View(nonStudentUsers);  
        }

        [HttpPost]
        public async Task<IActionResult> UpdateIsVedaStudent(string userId, bool isStudent)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            user.isStudent = isStudent;
            _context.Update(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true, userId = user.Id, isStudent = user.isStudent });
        }

        [HttpGet]
        public async Task<IActionResult>Edit(string id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var nonStudent = await _context.Users.FindAsync(id);
            if (nonStudent == null)
            {
                return NotFound();
            }
            NonVedaStudentViewModel nonStudentUpdate = new NonVedaStudentViewModel
            {
                UserId = nonStudent.Id,
                FirstName = nonStudent.FirstName,
                LastName = nonStudent.LastName,
                Email = nonStudent.Email,
                PhoneNumber = nonStudent.PhoneNumber,
                Gender = nonStudent.Gender,
                DateOfBirth = nonStudent.DateOfBirth,
                BranchId = nonStudent.BranchId,
                BoardId = nonStudent.BoardId,
                StandardId = nonStudent.StandardId,
            };
            ViewData["Board"] = new SelectList(_context.Boards, "Id", "Name", nonStudent.BoardId);
            ViewData["Branch"] = new SelectList(_context.Branches, "Id", "Name", nonStudent.BranchId);
            ViewData["Standard"] = new SelectList(_context.Standards,"Id","Name",nonStudent.StandardId);
            return View(nonStudentUpdate);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, NonVedaStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    user.isStudent = model.IsStudent;
                    user.BranchId = model.BranchId;
                    user.BoardId = model.BoardId;
                    user.StandardId = model.StandardId;

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "Non-Veda Student updated successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to update Non-Veda Student.";
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while updating the Non-Veda Student: " + ex.Message;
                }
            }

            if (ViewData["Board"]==null)
            {
                ViewData["Board"] = new SelectList(_context.Boards, "Id", "Name", model.BoardId);
            }

            if (ViewData["Branch"]==null)
            {
                ViewData["Branch"] = new SelectList(_context.Branches, "Id", "Name", model.BranchId);
            }

            if (ViewData["Standard"]==null)
            {
                ViewData["Standard"] = new SelectList(_context.Standards, "Id", "Name", model.StandardId);
            }
            return View(model);
        }


        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

    // POST: User/DeleteConfirmed/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        try
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "User has been removed.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception )
        {
            TempData["ErrorMessage"] = "An error occurred while deleting the user.";
            return RedirectToAction(nameof(Index));
        }
    }

        public bool userExists(string id)
        {
            return _userManager.Users.Any(e => e.Id == id);
        }

    }
}
