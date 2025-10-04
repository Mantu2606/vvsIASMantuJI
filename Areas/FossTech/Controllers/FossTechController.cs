using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FossTech.Areas.FossTech.ViewModels;
using FossTech.Data;
using System.Linq;
using System;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class FossTechController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FossTechController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;


            var model = new AdminViewModel
            {
                Posts = await _context.Posts.CountAsync(),
                Messages = await _context.ContactMessages.CountAsync(),
                Categories = await _context.Categories.CountAsync(),
                Products = await _context.Products.CountAsync(),
                UniqueVisitors = await _context.Visitors.CountAsync(),
                WhatsappClick = await _context.WhatsappClickCounts.CountAsync(),
                CallClick = await _context.CallClickCounts.CountAsync(),
                TotalVisitors = await _context.Visitors.CountAsync(),

                Enquiry = await _context.ContactMessages.CountAsync(),
                CoursesEnquiry = await _context.CoursesEnquiries.CountAsync(),
                FranchiseEnquiry = await _context.FranchiseEnquiries.CountAsync(),
                DemoLecturesEnquiry = await _context.DemoLecturesEnquiries.CountAsync(),
                
                Reviews = await _context.Reviews.CountAsync(),
                Testimonials = await _context.Testimonials.CountAsync(),
               
                Category = await _context.Categories.CountAsync(),
                SubCategory = await _context.SubCategories.CountAsync(),
               
                Blog = await _context.Posts.CountAsync(),
                Updates = await _context.Updates.CountAsync(),
                latestNews = await _context.LatestNews.CountAsync(),
                FAQs = await _context.FAQs.CountAsync(),
                CustomPages = await _context.Pages.CountAsync(),
               
                Menu = await _context.Menus.CountAsync(),
                Users = await _context.Users.CountAsync(),
                NewUsers = await _context.Users.Where(x => x.CreatedAt > Convert.ToDateTime($"01/{month}/{year}")).CountAsync(),
            };
            model.UniqueVisitors = await _context.Visitors.GroupBy(x => x.ClientIp).CountAsync();
            return View(model);
        }
    }
}