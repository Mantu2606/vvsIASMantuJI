using System.Threading.Tasks;
using FossTech.Data;
using FossTech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FossTech.ViewComponents
{
    [ViewComponent(Name = "SideSocialIcons")]
    public class SideSocialIcons : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SideSocialIcons(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var queryParams = _httpContextAccessor.HttpContext.Request.Query;
            var specificParam = queryParams["s"].ToString();

            var section = await _context.Sections.OrderBy(x => x.Id).FirstOrDefaultAsync();
            var sectionId = section.Id;

            if (!string.IsNullOrEmpty(specificParam))
            {
                var section1 = await _context.Sections.OrderBy(x => x.Id).FirstOrDefaultAsync();

                if (section1 != null)
                {
                    sectionId = section1.Id;
                }

            }
            var businessProfile = await _context.BusinessProfile.AnyAsync()
                ? await _context.BusinessProfile.FirstOrDefaultAsync()
                : new BusinessProfile();

            return View("Index", businessProfile);
        }
    }

}
