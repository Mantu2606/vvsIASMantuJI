using FossTech.Data;
using FossTech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace FossTech.Controllers
{
    public class TeachersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TeachersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string category)
        {
            ViewData["category"] = category;
            var team = await _context.Teams.OrderBy(x => x.SortOrder).ToListAsync();
            var director = await _context.OurDirectors.OrderBy(x => x.SortOrder).ToListAsync();
            var hod = await _context.OurHODs.OrderBy(x => x.SortOrder).ToListAsync();
            TeacherViewModel model = new TeacherViewModel
            {
                Team = team,
                Director = director,
                HOD = hod,
            };
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(model);
        }

        [Route("{controller}/{id}/{slug?}")]
        public async Task<IActionResult> Details(int id, string slug)
        {
            if (id == 0 || string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Where(x => x.Id == id && x.Slug == slug)
                .FirstOrDefaultAsync();

            var director = await _context.OurDirectors
                .Where(x => x.Id == id && x.Slug == slug)
                .FirstOrDefaultAsync();

            var hod = await _context.OurHODs
                .Where(x => x.Id == id && x.Slug == slug)
                .FirstOrDefaultAsync();

            if (team == null && director == null && hod == null)
            {
                return NotFound();
            }

            var model = new TeacherViewModel1
            {
                Team = team,
                Director = director,
                HOD = hod
            };

            return View(model);
        }


    }
    public class TeacherViewModel
    {
        public List<Team> Team { get; set; }
        public List<OurDirector> Director { get; internal set; }
        public List<OurHOD> HOD { get; set; }
        public WebSetting webSetting { get; set; }
    }public class TeacherViewModel1
    {
        public Team Team { get; set; }
        public OurDirector Director { get; internal set; }
        public OurHOD HOD { get; set; }
        public WebSetting webSetting { get; set; }
    }



}
