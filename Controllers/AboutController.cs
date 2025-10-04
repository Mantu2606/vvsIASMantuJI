using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FossTech.Data;
using FossTech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FossTech.Controllers
{
    public class AboutController : Controller
    {
        private readonly ILogger<AboutController> _logger;
        private readonly ApplicationDbContext _context;

        public AboutController(ILogger<AboutController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;

        }
        public async Task<IActionResult> Index()
        {
            var AboutUsList = await _context.AboutUs.ToListAsync();
            
            // Get the mission description specifically
            var missionContent = AboutUsList.FirstOrDefault(x => 
                x.Title.ToLower().Contains("mission") || 
                x.Slug.ToLower().Contains("mission") ||
                x.Title.ToLower().Contains("our mission") ||
                x.Slug.ToLower().Contains("our-mission"));
            
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            ViewData["MissionContent"] = missionContent;
            
            return View(AboutUsList);
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
            ViewBag.Testimonials = await _context.Testimonials.OrderByDescending(x => x.CreatedAt).Take(2).ToListAsync();
            ViewBag.BusinessProfile = await _context.BusinessProfile.FirstOrDefaultAsync();
            ViewBag.Team = await _context.Teams.ToListAsync();
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(about);
        }
    }
}