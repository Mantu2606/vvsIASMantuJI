using FossTech.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FossTech.Controllers
{
    public class DevelopedByController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DevelopedByController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var businessprofile = await _context.BusinessProfile.FirstOrDefaultAsync();
            return View(businessprofile);
        }
    }
}
