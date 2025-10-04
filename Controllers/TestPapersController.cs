using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models.TestPaperModels;
using FossTech.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FossTech.Controllers
{
    [Authorize]
    public class TestPapersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TestPapersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TestPapers
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            List<int> enrolledProductIds = _context.StudentCourses.Where(x => x.UserId == user.Id).Select(course => course.ProductId).ToList();

            var applicationDbContext = await _context.TestPapers
                .Where(testPaper => enrolledProductIds.Contains(testPaper.ProductId))
                .Include(x => x.PDFs)
                .Include(s => s.Product).ToListAsync();
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(applicationDbContext);
        }

        // GET: TestPapers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TestPapers == null)
            {
                return NotFound();
            }

            var testPaper = await _context.TestPapers
                .Include(t => t.Product)
                .Include(x => x.PDFs)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testPaper == null)
            {
                return NotFound();
            }
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(testPaper);
        }

        // GET: TestPapers/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: TestPapers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,SortOrder,ProductId,CreatedAt,LastModified")] TestPaper testPaper)
        {
            if (ModelState.IsValid)
            {
                _context.Add(testPaper);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", testPaper.ProductId);
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(testPaper);
        }

        // GET: TestPapers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TestPapers == null)
            {
                return NotFound();
            }

            var testPaper = await _context.TestPapers.FindAsync(id);
            if (testPaper == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", testPaper.ProductId);
            return View(testPaper);
        }

        // POST: TestPapers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,SortOrder,ProductId,CreatedAt,LastModified")] TestPaper testPaper)
        {
            if (id != testPaper.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(testPaper);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestPaperExists(testPaper.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", testPaper.ProductId);
            return View(testPaper);
        }

        // GET: TestPapers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TestPapers == null)
            {
                return NotFound();
            }

            var testPaper = await _context.TestPapers
                .Include(t => t.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testPaper == null)
            {
                return NotFound();
            }

            return View(testPaper);
        }

        // POST: TestPapers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TestPapers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TestPapers'  is null.");
            }
            var testPaper = await _context.TestPapers.FindAsync(id);
            if (testPaper != null)
            {
                _context.TestPapers.Remove(testPaper);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestPaperExists(int id)
        {
          return _context.TestPapers.Any(e => e.Id == id);
        }
    }
}
