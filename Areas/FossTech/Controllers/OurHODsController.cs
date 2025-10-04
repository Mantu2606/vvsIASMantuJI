using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    public class OurHODsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OurHODsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/OurHODs
        public async Task<IActionResult> Index()
        {
              return View(await _context.OurHODs
                  .Include(b => b.Section)
                  .OrderBy(x => x.SortOrder).ToListAsync());
        }

        // GET: FossTech/OurHODs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OurHODs == null)
            {
                return NotFound();
            }

            var ourHOD = await _context.OurHODs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ourHOD == null)
            {
                return NotFound();
            }

            return View(ourHOD);
        }

        // GET: FossTech/OurHODs/Create
        public IActionResult Create()
        {
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");

            return View();
        }

        // POST: FossTech/OurHODs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile Image, OurHOD ourHOD)
        {
            if (Image == null)
            {
                ModelState.AddModelError("Image", "Image is required.");
                return View(ourHOD);
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

            ourHOD.Image = $"/img/testimonial/{mainImageName}";

            if (ModelState.IsValid)
            {
                _context.Add(ourHOD);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");

            return View(ourHOD);
        }

        // GET: FossTech/OurHODs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OurHODs == null)
            {
                return NotFound();
            }

            var ourHOD = await _context.OurHODs.FindAsync(id);
            if (ourHOD == null)
            {
                return NotFound();
            }

            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");

            return View(ourHOD);
        }

        // POST: FossTech/OurHODs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile Image, OurHOD ourHOD)
        {
            if (id != ourHOD.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var thisMember = await _context.OurHODs.FirstOrDefaultAsync(i => i.Id == ourHOD.Id);
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

                    thisMember.Name = ourHOD.Name;
                    thisMember.Slug = ourHOD.Slug;
                    thisMember.Description = ourHOD.Description;
                    thisMember.Designation = ourHOD.Designation;
                    thisMember.Education = ourHOD.Education;
                    thisMember.Department = ourHOD.Department;
                    thisMember.SortOrder = ourHOD.SortOrder;
                    thisMember.SectionId = ourHOD.SectionId;

                    _context.Update(thisMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OurHODExists(ourHOD.Id))
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

            return View(ourHOD);
        }

        // GET: FossTech/OurHODs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OurHODs == null)
            {
                return NotFound();
            }

            var ourHOD = await _context.OurHODs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ourHOD == null)
            {
                return NotFound();
            }

            return View(ourHOD);
        }

        // POST: FossTech/OurHODs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OurHODs == null)
            {
                return Problem("Entity set 'ApplicationDbContext.OurHODs'  is null.");
            }
            var ourHOD = await _context.OurHODs.FindAsync(id);
            if (ourHOD != null)
            {
                _context.OurHODs.Remove(ourHOD);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ImageUpload(IFormFile file)
        {
            if (file != null)
            {
                //Set Key Name
                var imageName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/uploads")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/uploads"));
                }

                //Get url To Save
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/uploads", imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return Content($"/img/uploads/{imageName}");
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.OurHODs
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.OurHODs.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool OurHODExists(int id)
        {
          return _context.OurHODs.Any(e => e.Id == id);
        }
    }
}
