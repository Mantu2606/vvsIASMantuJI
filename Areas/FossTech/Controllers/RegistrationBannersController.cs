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
    public class RegistrationBannersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegistrationBannersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/RegistrationBanners
        public async Task<IActionResult> Index()
        {
              return View(await _context.RegistrationBanners
                  .Include(x => x.Section)
                  .ToListAsync());
        }

        // GET: FossTech/RegistrationBanners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.RegistrationBanners == null)
            {
                return NotFound();
            }

            var registrationBanner = await _context.RegistrationBanners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (registrationBanner == null)
            {
                return NotFound();
            }

            return View(registrationBanner);
        }

        // GET: FossTech/RegistrationBanners/Create
        public IActionResult Create()
        {
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");

            return View();
        }

        // POST: FossTech/RegistrationBanners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistrationBanner registrationBanner , IFormFile mainImage)
        {
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");

            if (ModelState.IsValid)
            {
                if (mainImage == null)
                {
                    ModelState.AddModelError("Image", "Registration banner Image is required.");
                    return View(registrationBanner);
                }
                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/RegistorBanner")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot/img/RegistorBanner"));
                }
                var mainImageName = Guid.NewGuid() + Path.GetExtension(mainImage.FileName);

                //Get url To Save
                var mainImageSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/RegistorBanner", mainImageName);

                await using (var stream = new FileStream(mainImageSavePath, FileMode.Create))
                {
                    await mainImage.CopyToAsync(stream);
                }
                registrationBanner.Image = $"/img/RegistorBanner/{mainImageName}";
                _context.Add(registrationBanner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
                return View(registrationBanner);
            }
        
        // GET: FossTech/RegistrationBanners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.RegistrationBanners == null)
            {
                return NotFound();
            }

            var registrationBanner = await _context.RegistrationBanners.FindAsync(id);
            if (registrationBanner == null)
            {
                return NotFound();
            }

            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");

            return View(registrationBanner);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  RegistrationBanner registrationBanner, IFormFile mainImage)
        {
            if (id != registrationBanner.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldRegistrationBanner = await _context.RegistrationBanners.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == id);

                    if (mainImage != null)
                    {
                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/RegistorBanner")))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                                "wwwroot/img/RegistorBanner"));
                        }
                        var mainImageName = Guid.NewGuid() + Path.GetExtension(mainImage.FileName);

                        //Get url To Save
                        var mainImageSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/RegistorBanner", mainImageName);

                        await using (var stream = new FileStream(mainImageSavePath, FileMode.Create))
                        {
                            await mainImage.CopyToAsync(stream);
                        }
                        oldRegistrationBanner.Image = $"/img/RegistorBanner/{mainImageName}";
                    }
                   
                    oldRegistrationBanner.SectionId = registrationBanner.SectionId;
                    oldRegistrationBanner.LastModified = registrationBanner.LastModified;

                    _context.Update(oldRegistrationBanner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistrationBannerExists(registrationBanner.Id))
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

            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");
            return View(registrationBanner);
        }

        // GET: FossTech/RegistrationBanners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.RegistrationBanners == null)
            {
                return NotFound();
            }

            var registrationBanner = await _context.RegistrationBanners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (registrationBanner == null)
            {
                return NotFound();
            }

            return View(registrationBanner);
        }

        // POST: FossTech/RegistrationBanners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.RegistrationBanners == null)
            {
                return Problem("Entity set 'ApplicationDbContext.RegBanner'  is null.");
            }
            var registrationBanner = await _context.RegistrationBanners.FindAsync(id);
            if (registrationBanner != null)
            {
                _context.RegistrationBanners.Remove(registrationBanner);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegistrationBannerExists(int id)
        {
          return _context.RegistrationBanners.Any(e => e.Id == id);
        }
    }
}
