using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models.StudentModels;
using Microsoft.AspNetCore.Identity;
using FossTech.Models;
using FossTech.Areas.Dashboard.ViewModels;

namespace FossTech.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class PurchasesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PurchasesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userId = currentUser?.Id;

            // Get Study Material purchases
            var userStudyMaterialAccesses = await _context.UserStudyMaterialAccesses
                .Include(u => u.StudyMaterial)
                .ThenInclude(x => x.Board)
                .Include(u => u.StudyMaterial)
                .ThenInclude(x => x.Subject)
                .Include(u => u.StudyMaterial)
                .ThenInclude(x => x.Standard)
                .Where(u => u.User.Id == userId)
                .OrderByDescending(u => u.PurchasedAt)
                .ToListAsync();

            // Get Flash Card purchases
            var userFlashCardAccesses = await _context.UserFlashCardAccessess
                .Include(u => u.FlashCard)
                .ThenInclude(x => x.Board)
                .Include(u => u.FlashCard)
                .ThenInclude(x => x.Subject)
                .Include(u => u.FlashCard)
                .ThenInclude(x => x.Standard)
                .Where(u => u.User.Id == userId)
                .OrderByDescending(u => u.PurchasedAt)
                .ToListAsync();

            // Create view model with both types of data
            var viewModel = new PurchaseHistoryViewModel
            {
                StudyMaterialAccesses = userStudyMaterialAccesses,
                FlashCardAccesses = userFlashCardAccesses
            };

            return View(viewModel);
        }
    }
}
