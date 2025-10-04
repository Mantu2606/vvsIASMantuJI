using System.Threading.Tasks;
using FossTech.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FossTech.Controllers
{
    public class PageController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PageController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Route("[controller]/{Slug}")]
        public async Task<IActionResult> Details(string Slug)
        {
            if (Slug == null)
            {
                return NotFound();
            }

            var page = await _context.Pages
                .FirstOrDefaultAsync(m => m.Slug == Slug);
            if (page == null)
            {
                return NotFound();
            }
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(page);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Pages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (page == null)
            {
                return NotFound();
            }
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(page);
        }
    }
}