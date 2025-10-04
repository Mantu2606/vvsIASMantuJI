using System.Linq;
using System.Threading.Tasks;
using FossTech.Data;
using FossTech.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FossTech.ViewComponents
{
    [ViewComponent(Name = "ChatBot")]
    public class ChatBot: ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpContextAccessor _httpContextAccessor;

        public ChatBot(ApplicationDbContext context, HttpContextAccessor httpContextAccessor)
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

            var businessProfile = new Models.BusinessProfile();
            if (_context.BusinessProfile.Any())
            {
                 businessProfile = await _context.BusinessProfile.Where(x => x.SectionId == sectionId).FirstOrDefaultAsync();
            }
           

            NavViewModel model = new NavViewModel
            {



                BusinessProfile = businessProfile,


            };

            return View("Index", model);
        }
    }
}
