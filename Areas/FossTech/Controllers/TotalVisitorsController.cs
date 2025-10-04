using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Extensions;
using FossTech.Models;
using System;
using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class TotalVisitorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TotalVisitorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? StartDate, DateTime? EndDate)
        {
            StartDate ??= DateTime.Now.AddMonths(-1);
            EndDate ??= DateTime.Now;
            var TotalVisitors = await _context.Visitors
                                              .Where(x => x.CreatedAt.Date >= StartDate.Value.Date && x.CreatedAt.Date <= EndDate.Value.Date)
                                              .GroupBy(x => x.CreatedAt.Date)
                                              .Select(x => new TotalVisitorViewmodel
                                              {
                                                  Date = x.Key,
                                                  Count = x.Count()
                                              })
                                              .OrderBy(x => x.Date)
                                              .ToListAsync();
            ViewData["StartDate"] = StartDate.Value.ToString("yyy-MM-dd");
            ViewData["EndDate"] = EndDate.Value.ToString("yyy-MM-dd");
            return View(TotalVisitors);
        }

        public async Task<IActionResult> DownloadData(DateTime? StartDate, DateTime? EndDate)
        {
            StartDate ??= DateTime.Now.AddMonths(-1);
            EndDate ??= DateTime.Now;

            var totalVisitors = await _context.Visitors
                .Where(x => x.CreatedAt.Date >= StartDate.Value.Date && x.CreatedAt.Date <= EndDate.Value.Date)
                .OrderByDescending(x => x.CreatedAt)
                .GroupBy(x => x.CreatedAt.Date)
                .Select(x => new TotalVisitorViewmodel
                {
                    Date = x.Key,
                    Count = x.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var csvContent = "Date,Visitor Count\n";
            foreach (var item in totalVisitors)
            {
                csvContent += $"{item.Date.ToString("yyyy-MM-dd")},{item.Count}\n";
            }

            var fileBytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
            return File(fileBytes, "text/csv", "totalvisitors.csv");
        }

    }
    public class TotalVisitorViewmodel
    {
        public DateTime Date { get; set; }
        public int? Count { get; set; }

    }
}
