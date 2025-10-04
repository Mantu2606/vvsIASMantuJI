using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    public class BusinessProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BusinessProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/BusinessProfiles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BusinessProfile.Include(b => b.Section);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FossTech/BusinessProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BusinessProfile == null)
            {
                return NotFound();
            }

            var businessProfile = await _context.BusinessProfile
                .Include(b => b.Section)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (businessProfile == null)
            {
                return NotFound();
            }

            return View(businessProfile);
        }

        // GET: FossTech/BusinessProfiles/Create
        public IActionResult Create()
        {
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");
            return View();
        }

        // POST: FossTech/BusinessProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BusinessName,Title,EmailAddress,AlternateEmailAddress,AlternateEmailAddress1,RegisteredContactNumber,AlternateContactNumber,AlternateContactNumber1,WhatsAppNumber,BusinessWhatsAppNumber,BusinessWhatsAppNumber1,FacebookPageLink,InstagramPageLink,TwitterLink,LinkedInLink,YoutubeLink,OtherWebsite,WorkingHours,Copyright,CreatedAt,LastModified,SectionId")] BusinessProfile businessProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(businessProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name", businessProfile.SectionId);
            return View(businessProfile);
        }

        // GET: FossTech/BusinessProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BusinessProfile == null)
            {
                return NotFound();
            }

            var businessProfile = await _context.BusinessProfile.FindAsync(id);
            if (businessProfile == null)
            {
                return NotFound();
            }
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name", businessProfile.SectionId);
            return View(businessProfile);
        }

        // POST: FossTech/BusinessProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BusinessName,Title,EmailAddress,AlternateEmailAddress,AlternateEmailAddress1,RegisteredContactNumber,AlternateContactNumber,AlternateContactNumber1,WhatsAppNumber,BusinessWhatsAppNumber,BusinessWhatsAppNumber1,FacebookPageLink,InstagramPageLink,TwitterLink,LinkedInLink,YoutubeLink,OtherWebsite,WorkingHours,Copyright,CreatedAt,LastModified,GoogleMap,SectionId")] BusinessProfile businessProfile)
        {
            if (id != businessProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(businessProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusinessProfileExists(businessProfile.Id))
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
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name", businessProfile.SectionId);
            return View(businessProfile);
        }

        // GET: FossTech/BusinessProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BusinessProfile == null)
            {
                return NotFound();
            }

            var businessProfile = await _context.BusinessProfile
                .Include(b => b.Section)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (businessProfile == null)
            {
                return NotFound();
            }

            return View(businessProfile);
        }

        // POST: FossTech/BusinessProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BusinessProfile == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BusinessProfile'  is null.");
            }
            var businessProfile = await _context.BusinessProfile.FindAsync(id);
            if (businessProfile != null)
            {
                _context.BusinessProfile.Remove(businessProfile);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BusinessProfileExists(int id)
        {
          return _context.BusinessProfile.Any(e => e.Id == id);
        }
    }
}
