using FossTech.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace FossTech.Controllers
{
    public class ResultsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ResultsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string topperYear, int? sectionId)
        {
            var query = _context.OurToppers.AsQueryable();

            if (!string.IsNullOrEmpty(topperYear))
            {
                query = query.Where(x => x.TopperYear == topperYear);
            }

            if (sectionId.HasValue)
            {
                query = query.Where(x => x.SectionId == sectionId);
            }

            var testim = await query.OrderBy(x => x.SortOrder).ToListAsync();

            ViewData["TopperYears"] = new SelectList(await _context.OurToppers
                .Select(x => x.TopperYear)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync(), topperYear);

            ViewData["Sections"] = new SelectList(await _context.Sections
                .Select(s => new { s.Id, s.Name })
                .OrderBy(s => s.Name)
                .ToListAsync(), "Id", "Name", sectionId);

            return View(testim);
        }
    }
}
