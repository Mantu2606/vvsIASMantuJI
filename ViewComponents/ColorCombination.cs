using System.Linq;
using System.Threading.Tasks;
using FossTech.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FossTech.ViewComponents
{
    [ViewComponent(Name = "ColorCombination")]
    public class ColorCombination : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public ColorCombination(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var colorcombination = new Models.ColorCombination();
            if (_context.ColorCombinations.Any())
            {
                colorcombination = await _context.ColorCombinations.FirstOrDefaultAsync();
            }

            return View("Index", colorcombination);
        }
    }
}
