using FossTech.Data;
using FossTech.Extensions;
using FossTech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FossTech.Controllers
{
    public class FranchiseController : Controller
    {
        private readonly ApplicationDbContext _context;
        public FranchiseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var franchise = await _context.Franchises.FirstOrDefaultAsync();
            var franchiseEnquiry = await _context.FranchiseEnquiries.FirstOrDefaultAsync();

            FranchiseViewModel model = new FranchiseViewModel { 
                Franchise = franchise,
                FrachiseEnquiry = franchiseEnquiry,
            };
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(FranchiseEnquiry franchiseEnquiry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(franchiseEnquiry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)).WithSuccess("Your Form Has Submitted Successfully We Will Call You Shortly", "Your Form Has Submitted Successfully We Will Call You Shortly");
            }
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(franchiseEnquiry).WithError("Error");
        }
    }
    public class FranchiseViewModel
    {
        public Franchise Franchise { get;  set; }
        public FranchiseEnquiry FrachiseEnquiry { get;  set; }
        [Required]
        [DisplayName("Full Name")]
        public string FullName { get; set; }
        [Required]
        [DisplayName("Mobile Number")]
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Pincode { get; set; }
    }
}
