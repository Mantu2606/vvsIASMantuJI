using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    public class PageVisitorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PageVisitorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/PageVisitors
        public async Task<IActionResult> Index()
        {
            // Group visitors by FullURL and get counts
            var urlVisitorCounts = await _context.Visitors
                .Where(x => x.CreatedAt.Year == DateTime.Now.Year)
                .GroupBy(x => x.FullURL)
                .Select(x => new PageVisitorViewModel
                {
                    URL = x.Key,
                    Count = x.Count()
                })
                .ToListAsync();

            // Pass the data to the view
            return View(urlVisitorCounts);
        }


    }

    public class PageVisitorViewModel
    {
        public string URL { get; set; }
        public int Count { get; set; }
    }
}


