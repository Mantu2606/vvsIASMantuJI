using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using FossTech.Data;
using FossTech.Models;
using Fingers10.ExcelExport.ActionResults;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class TestimonialsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestimonialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/testimonials
        public async Task<IActionResult> Index()
        {
            return View(await _context.Testimonials
                .Include(b => b.Section)
                .OrderBy(x => x.Order)
                .ToListAsync());
        }

        public async Task<IActionResult> ExporttoExcel()
        {
            var results = await _context.Testimonials.ToListAsync();
            return new ExcelResult<Testimonial>(results, "Demo Sheet Name", "Fingers10");
        }


        // GET: FossTech/testimonials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        // GET: FossTech/testimonials/Create
        public IActionResult Create()
        {
            ViewData["Sections"] = new MultiSelectList(_context.Sections, "Id", "Name");
            return View();
        }

        // POST: FossTech/testimonials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile Image, Testimonial testimonial)
        {
            
            if (Image == null)
            {
                ModelState.AddModelError("Image", "Image is required.");
                return View(testimonial);
            }
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/testimonial")))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot/img/testimonial"));
            }
            var mainImageName = Guid.NewGuid() + Path.GetExtension(Image.FileName);

            //Get url To Save
            var mainImageSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/testimonial", mainImageName);

            await using (var stream = new FileStream(mainImageSavePath, FileMode.Create))
            {
                await Image.CopyToAsync(stream);
            }

            testimonial.Image = $"/img/testimonial/{mainImageName}";

            if (ModelState.IsValid)
            {
                _context.Add(testimonial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Sections"] = new MultiSelectList(_context.Sections, "Id", "Name");
            return View(testimonial);
        }

        // GET: FossTech/testimonials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }
            ViewData["Sections"] = new MultiSelectList(_context.Sections, "Id", "Name");
            return View(testimonial);
        }

        // POST: FossTech/testimonials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile Image, Testimonial testimonial)
        {
            ViewData["Sections"] = new MultiSelectList(_context.Sections, "Id", "Name");
            if (id != testimonial.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var thisMember = await _context.Testimonials.FirstOrDefaultAsync(i => i.Id == testimonial.Id);
                    if (thisMember == null)
                    {
                        return NotFound();
                    }

                    if (Image != null)
                    {
                        var mainImageName = Guid.NewGuid() + Path.GetExtension(Image.FileName);

                        //Get url To Save
                        var mainImageSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/testimonial", mainImageName);

                        await using (var stream = new FileStream(mainImageSavePath, FileMode.Create))
                        {
                            await Image.CopyToAsync(stream);
                        }

                        thisMember.Image = $"/img/testimonial/{mainImageName}";
                    }
                    thisMember.Name = testimonial.Name;
                    thisMember.Review = testimonial.Review;
                    thisMember.Order = testimonial.Order;
                    thisMember.Slug = testimonial.Slug;
                    thisMember.SectionId = testimonial.SectionId;

                    _context.Update(thisMember);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!testimonialExists(testimonial.Id))
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
            return View(testimonial);
        }

        // GET: FossTech/testimonials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        // POST: FossTech/testimonials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            _context.Testimonials.Remove(testimonial);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.Testimonials
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.Testimonials.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool testimonialExists(int id)
        {
            return _context.Testimonials.Any(e => e.Id == id);
        }
    }
}
