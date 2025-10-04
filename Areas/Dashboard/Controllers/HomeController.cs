using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using FossTech.Areas.Dashboard.ViewModels;
using FossTech.Data;
using FossTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FossTech.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize(Roles = "User,OutSideUser")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context,UserManager<ApplicationUser> userManager) 
        {
            _context = context; 
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var month = System.DateTime.Now.Month;
              var year = System.DateTime.Now.Year;
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            var model = new UserViewModel
            {

                StudyMaterial = await _context.StudyMaterials.CountAsync(),
                UserName = user?.UserName,

            };
            return View(model);
        }
    }
}
       