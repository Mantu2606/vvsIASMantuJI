using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FossTech.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Models.ProductModels;
using X.PagedList;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using FossTech.Extensions;

namespace FossTech.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            // Trim the keyword and convert it to lowercase
            keyword = keyword.Trim().ToLower();

            var services = await _context.Products
                .Where(s => s.Name.ToLower().Contains(keyword))
                .ToListAsync();

            var result = new
            {
                Services = services,
            };

            return Json(result);
        }

        public async Task<IActionResult> Index(string keyword)
        {
           
            var serviceResult = await _context.Products
                .Where(s => s.Name.ToLower().Contains(keyword))
                .FirstOrDefaultAsync();

            if (serviceResult != null)
            {
                // Redirect to the service detail page with "services/{id}" format
                return Redirect($"/courses/{serviceResult.Id}");
            }
            else
            {
                // Handle case when no matching results are found, perhaps show a message or a different view
                return RedirectToAction("Index", "Home").WithInfo("No Result Found Search What Inside The Search List");
            }
        }


    }
}