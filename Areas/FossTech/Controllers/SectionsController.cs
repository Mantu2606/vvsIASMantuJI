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
    public class SectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/Sections
        public async Task<IActionResult> Index()
        {
            
            return View(await _context.Sections.OrderBy(s => s.SortOrder).ToListAsync());
        }

        // GET: FossTech/Sections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Sections == null)
            {
                return NotFound();
            }

            var section = await _context.Sections
                .FirstOrDefaultAsync(m => m.Id == id);
            if (section == null)
            {
                return NotFound();
            }

            return View(section);
        }

        // GET: FossTech/Sections/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FossTech/Sections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Section section, IFormFile mainImage)
        {
            if (ModelState.IsValid)
            {
                if (mainImage == null)
                {
                    ModelState.AddModelError("Image", "Section Image is required.");
                    return View(section);
                }
                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/sections")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot/img/sections"));
                }
                var mainImageName = Guid.NewGuid() + Path.GetExtension(mainImage.FileName);

                //Get url To Save
                var mainImageSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/sections", mainImageName);

                await using (var stream = new FileStream(mainImageSavePath, FileMode.Create))
                {
                    await mainImage.CopyToAsync(stream);
                }
            
                    section.Image = $"/img/sections/{mainImageName}";
                    _context.Add(section);
                    await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(section);
        }

        // GET: FossTech/Sections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Sections == null)
            {
                return NotFound();
            }

            var section = await _context.Sections.FindAsync(id);
            if (section == null)
            {
                return NotFound();
            }
            return View(section);
        }

        // POST: FossTech/Sections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile mainImage, Section section)
        {
            var thisPost = await _context.Sections.FirstOrDefaultAsync(p => p.Id == section.Id);

            if (thisPost == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(section);
            }

            thisPost.Name = section.Name;
            thisPost.SortOrder = section.SortOrder;

            if (mainImage != null)
            {
                var imageName = Guid.NewGuid() + Path.GetExtension(mainImage.FileName);
                var imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/section");
                if (!Directory.Exists(imageDirectory))
                {
                    Directory.CreateDirectory(imageDirectory);
                }

                var savePath = Path.Combine(imageDirectory, imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await mainImage.CopyToAsync(stream);
                }

                thisPost.Image = $"/img/section/{imageName}";
            }

            _context.Update(thisPost);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: FossTech/Sections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Sections == null)
            {
                return NotFound();
            }

            var section = await _context.Sections
                .FirstOrDefaultAsync(m => m.Id == id);
            if (section == null)
            {
                return NotFound();
            }

            return View(section);
        }

        // POST: FossTech/Sections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Sections == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Sections'  is null.");
            }
            var section = await _context.Sections.FindAsync(id);
            if (section != null)
            {
                _context.Sections.Remove(section);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SectionExists(int id)
        {
          return _context.Sections.Any(e => e.Id == id);
        }
    }
}
