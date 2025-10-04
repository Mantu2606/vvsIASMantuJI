using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models.StudyMaterialModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FossTech.Models;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class FeedBackFormsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FeedBackFormsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: FossTech/FeedBackForms
        public async Task<IActionResult> Index()
        {

            var feedback = await _context.FeedBackForms.Include(f => f.User).ToListAsync();
            return View(feedback);
        }

        // GET: FossTech/FeedBackForms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FeedBackForms == null)
            {
                return NotFound();
            }

            var feedBackForm = await _context.FeedBackForms
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedBackForm == null)
            {
                return NotFound();
            }

            return View(feedBackForm);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FeedBackForms == null)
            {
                return NotFound();
            }

            var feedBackForm = await _context.FeedBackForms.FindAsync(id);
            if (feedBackForm == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", feedBackForm.UserId);
            return View(feedBackForm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FeedBackForm feedBackForm)
        {
            if (id != feedBackForm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedBackForm);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedBackFormExists(feedBackForm.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", feedBackForm.UserId);
            return View(feedBackForm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsSolved(int id)
        {
            var feedback = await _context.FeedBackForms.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            feedback.IsSolved = true;
            feedback.SolvedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsUnsolved(int id)
        {
            var feedbackForm = await _context.FeedBackForms.FindAsync(id);
            if (feedbackForm == null)
            {
                return NotFound();
            }

            feedbackForm.IsSolved = false;

            _context.Update(feedbackForm);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FeedBackForms == null)
            {
                return NotFound();
            }

            var feedBackForm = await _context.FeedBackForms
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedBackForm == null)
            {
                return NotFound();
            }

            return View(feedBackForm);
        }

        // POST: FossTech/FeedBackForms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FeedBackForms == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FeedBackForms'  is null.");
            }
            var feedBackForm = await _context.FeedBackForms.FindAsync(id);
            if (feedBackForm != null)
            {
                _context.FeedBackForms.Remove(feedBackForm);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedBackFormExists(int id)
        {
            return _context.FeedBackForms.Any(e => e.Id == id);
        }
    }
}
