using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models.StudyMaterialModels;
using FossTech.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FossTech.Controllers
{
    [Authorize]
    public class StudyMaterialsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudyMaterialsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: StudyMaterials
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            List<int> enrolledProductIds = _context.StudentCourses.Where(x => x.UserId == user.Id).Select(course => course.ProductId).ToList();

            var query = _context.StudyMaterials
                .Include(x => x.PDFs)
                .Include(s => s.Product)
                .Include(s => s.Board)
                .Include(s => s.Subject)
                .Include(s => s.Standard)
                .AsQueryable();

            // If user is not a student, show only paid materials
            if (!user.isStudent)
            {
                query = query.Where(sm => sm.IsPaid == true);
            }

            var applicationDbContext = await query.ToListAsync();
            
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(applicationDbContext);
        }

        // GET: StudyMaterials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StudyMaterials == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            var query = _context.StudyMaterials
                .Include(s => s.Product)
                .Include(x => x.PDFs)
                .Include(s => s.Board)
                .Include(s => s.Subject)
                .Include(s => s.Standard)
                .AsQueryable();

            // If user is not a student, show only paid materials
            if (!user.isStudent)
            {
                query = query.Where(sm => sm.IsPaid == true);
            }

            var studyMaterial = await query.FirstOrDefaultAsync(m => m.Id == id);
            
            if (studyMaterial == null)
            {
                return NotFound();
            }
            
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(studyMaterial);
        }

    }
}
