using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FossTech.Data;
using FossTech.Models;
using FossTech.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FossTech.ViewComponents
{
    [ViewComponent(Name = "Footer")]
    public class Footer : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Footer(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var queryParams = _httpContextAccessor.HttpContext.Request.Query;
            var specificParam = queryParams["s"].ToString();


            var businessProfile = new Models.BusinessProfile();
            var logo = new Logo();
            var aboutUs = new AboutUs();
            List<Branch> branches = new List<Branch>();


            var section = await _context.Sections.OrderBy(x => x.Id).FirstOrDefaultAsync();

            if (section != null) {
                var sectionId = section.Id;

                if (!string.IsNullOrEmpty(specificParam))
                {
                    var section1 = await _context.Sections.OrderBy(x => x.Id).FirstOrDefaultAsync();

                    if (section1 != null)
                    {
                        sectionId = section1.Id;
                    }
                }

                if (await _context.BusinessProfile.Where(x => x.SectionId == sectionId).AnyAsync())
                {
                    businessProfile = await _context.BusinessProfile.Where(x => x.SectionId == sectionId).FirstOrDefaultAsync();
                }

                if (await _context.AboutUs.Where(x => x.SectionId == sectionId).AnyAsync())
                {
                    aboutUs = await _context.AboutUs.Where(x => x.SectionId == sectionId).FirstOrDefaultAsync();
                }

                branches = await _context.Branches.Where(x => x.SectionId == sectionId).OrderBy(x => x.SortOrder).ToListAsync();
            }

            if (await _context.Logos.AnyAsync())
            {
                logo = await _context.Logos.FirstOrDefaultAsync();
            }
           
            var websetting = await _context.WebSettings.FirstOrDefaultAsync();
            if (websetting == null)
            {
                websetting = new WebSetting();
            }

            var menus = await _context.Menus.Where(x => x.ShowInFooter).Include(m => m.MenuProducts).ThenInclude(p => p.Product).Include(c => c.MenuCategories).ThenInclude(c => c.Category).OrderBy(o => o.Order).ToListAsync();
           
            NavViewModel model = new NavViewModel
            {
                BusinessProfile = businessProfile,
                Logo = logo,
                AboutUs = aboutUs,
                WebSetting = websetting,
                Pages = await _context.Pages.OrderBy(x => x.Order).ToListAsync(),
                Branches = branches,
                KeyWords = await _context.KeyWords.OrderBy(x => x.SortOrder).ToListAsync(),
                ShowFooter = menus,
                Categories = await _context.Categories.ToListAsync(),
                Products=await _context.Products.ToListAsync(),
                Sections = await _context.Sections.OrderBy(x => x.Name).ToListAsync()
            };

            return View("Index", model);
        }
    }
}