using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models.StudyMaterialModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using FossTech.Models;

namespace FossTech.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize(Roles = "User")]
    public class FeedBackFormsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FeedBackFormsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);
            if (currentUser==null)
            {
                return NotFound();
            }
            var userFeedbackForms = await _context.FeedBackForms.Where(f => f.UserId == currentUser.Id).ToListAsync();

            return View(userFeedbackForms);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FeedBackForms == null)
            {
                return NotFound();
            }

            var feedBackForm = await _context.FeedBackForms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedBackForm == null)
            {
                return NotFound();
            }

            return View(feedBackForm);
        }

        // GET: Dashboard/FeedBackForms/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FeedBackForm feedBackForm, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                string imageName = null;

                if (image != null)
                {
                    imageName = Guid.NewGuid() + Path.GetExtension(image.FileName);

                    var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "Feedback");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    var savePath = Path.Combine(uploadDir, imageName);

                    try
                    {
                        await using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        feedBackForm.Image = $"/img/Feedback/{imageName}";
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "There was an issue uploading the image: " + ex.Message);
                        return View(feedBackForm);
                    }
                }
                ApplicationUser currentUser = await _userManager.GetUserAsync(User);
                feedBackForm.UserId = currentUser?.Id;
                _context.Add(feedBackForm);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(feedBackForm);
        }

        private bool FeedBackFormExists(int id)
        {
            return _context.FeedBackForms.Any(e => e.Id == id);
        }
    }
}
