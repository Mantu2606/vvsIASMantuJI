using System.Linq;
using System.Threading.Tasks;
using FossTech.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FossTech.ViewModels;

namespace FossTech.ViewComponents
{
    [ViewComponent(Name = "TopBar")]
    public class TopBar : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public TopBar(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var businessProfile = await _context.BusinessProfile.AnyAsync()
                ? await _context.BusinessProfile.FirstOrDefaultAsync()
                : new FossTech.Models.BusinessProfile();

            //var categories = await _context.Categories.Include(s => s.SubCategories).OrderByDescending(k => k.SubCategories.Count).ToListAsync();

            TopBarViewModel model = new TopBarViewModel
            {

                BusinessProfile = businessProfile,
                //Categories=categories,
            };
            
            return View("Index", model);
        }


        
    }




    
}