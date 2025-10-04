using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FossTech.Data;
using FossTech.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FossTech.Controllers
{
    public class FAQsController : Controller
    {
        private readonly ILogger<FAQsController> _logger;
        private readonly ApplicationDbContext _context;

        public FAQsController(ILogger<FAQsController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;

        }
        public async Task<IActionResult> Index()
        {
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");

            return View(await _context.FAQs.ToListAsync());
        }

        public class AboutViewModel
        {
            public List<Image> Images { get; set; }
            public List<AboutUs> AboutUsList { get; set; }
        }

        [Route("[Controller]/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            if (slug == null)
            {
                return NotFound();
            }

            var about = await _context.AboutUs
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (about == null)
            {
                return NotFound();
            }

            ViewBag.Team = await _context.Teams.ToListAsync();
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(about);
        }
    }
}