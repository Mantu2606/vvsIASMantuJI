using FossTech.Areas.FossTech.ViewModels;
using FossTech.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class UniqueVisitorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UniqueVisitorsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var uniqueVisitors = await _context.Visitors
                 .GroupBy(v => v.CreatedAt.Date)
                 .Select(g => new
                 {
                     Date = g.Key,
                     Count = g.Count()
                 })
                 .OrderBy(x => x.Date)
                 .ToListAsync();

            var viewModel = uniqueVisitors.Select(u => new UniqueVisitorViewModel
            {
                Date = u.Date,
                VisitorCount = u.Count
            }).ToList();

            return View(viewModel);
        }
    }
}
