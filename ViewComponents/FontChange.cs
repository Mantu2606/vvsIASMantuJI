using System.Linq;
using System.Threading.Tasks;
using FossTech.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FossTech.ViewComponents
{
    [ViewComponent(Name = "FontChange")]
    public class FontChange : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public FontChange(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var fontChange = new Models.FontChange();
            if (_context.FontChanges.Any())
            {
                fontChange = await _context.FontChanges.FirstOrDefaultAsync();
            }

            return View("Index", fontChange);
        }
    }
}
