using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FossTech.Data;
using FossTech.Models;
using FossTech.Models.Menu;
using FossTech.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FossTech.ViewComponents
{
    [ViewComponent(Name = "Navigation")]
    public class Navigation :ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;

        public Navigation(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var queryParams = _httpContextAccessor.HttpContext.Request.Query;
            var specificParam = queryParams["s"].ToString();

            List<Page> page = new List<Page>();
            List<Models.Category> categories = new List<Models.Category>();
            BusinessProfile businessProfile = new BusinessProfile();

            List<AboutUs> aboutList = new List<AboutUs>();

            var section = await _context.Sections.OrderBy(x => x.Id).FirstOrDefaultAsync();
            if (section != null)
            {
                var sectionId = section.Id;
                if (!string.IsNullOrEmpty(specificParam))
                {
                    var section1 = await _context.Sections.OrderBy(x => x.Id).FirstOrDefaultAsync();

                    if (section1 != null)
                    {
                        sectionId = section1.Id;
                    }
                }


                businessProfile = await _context.BusinessProfile.AnyAsync()
                    ? await _context.BusinessProfile.Where(x => x.SectionId == sectionId).FirstOrDefaultAsync()
                    : new BusinessProfile();


                page = await _context.Pages.OrderBy(o => o.Order).ToListAsync(); 
                categories = await _context.Categories.Where(x => x.SectionId == sectionId).Include(s => s.SubCategories).OrderByDescending(k => k.SubCategories.Count).ToListAsync();

                aboutList = await _context.AboutUs.Where(x => x.SectionId == sectionId).OrderBy(x => x.SortOrder).ToListAsync();
            }




            var logo = await _context.Logos.AnyAsync()
                ? await _context.Logos.FirstOrDefaultAsync()
                : new Logo();


            //var products = await _context.Products.ToListAsync();

            List<Menu> menus = await _context.Menus.Where(x => x.ShowInHeader).Include(m => m.MenuProducts).ThenInclude(p => p.Product).Include(m => m.MenuPages).ThenInclude(p => p.Page).Include(c => c.MenuCategories).ThenInclude(c => c.Category).OrderBy(o => o.Order).ToListAsync();
            var websetting = await _context.WebSettings.FirstOrDefaultAsync();
            if (websetting == null)
            {
                websetting = new WebSetting();
            }

            NavViewModel model = new NavViewModel
            {
                Menus = menus,
                Logo = logo,
                More = page,
                WebSetting = websetting,
                Categories = categories,
                BusinessProfile = businessProfile,
                //Products = products,
                AboutsList =  aboutList,
            };
            
            return View("Index", model);
        }

    }
}
