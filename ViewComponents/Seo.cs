using System.Linq;
using System.Threading.Tasks;
using FossTech.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FossTech.ViewComponents
{
    [ViewComponent(Name = "Seo")]
    public class Seo: ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public Seo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var seo = new Models.Seo();
            if (_context.Seo.Any())
            {
                seo = await _context.Seo.FirstOrDefaultAsync();
            }
          
            return View("Index", seo);
        }
    }
}
