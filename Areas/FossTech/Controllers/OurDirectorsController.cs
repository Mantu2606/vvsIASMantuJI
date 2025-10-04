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
    public class OurDirectorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OurDirectorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/OurDirectors
        public async Task<IActionResult> Index()
        {
              return View(await _context.OurDirectors
                  .Include(b => b.Section)
                  .OrderBy(x => x.SortOrder).ToListAsync());
        }

        // GET: FossTech/OurDirectors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OurDirectors == null)
            {
                return NotFound();
            }

            var ourDirector = await _context.OurDirectors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ourDirector == null)
            {
                return NotFound();
            }

            return View(ourDirector);
        }

        // GET: FossTech/OurDirectors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FossTech/OurDirectors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile Image, OurDirector ourDirector)
        {
            if (Image == null)
            {
                ModelState.AddModelError("Image", "Image is required.");
                return View(ourDirector);
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

            ourDirector.Image = $"/img/testimonial/{mainImageName}";

            if (ModelState.IsValid)
            {
                _context.Add(ourDirector);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");

            return View(ourDirector);
        }

        // GET: FossTech/OurDirectors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OurDirectors == null)
            {
                return NotFound();
            }

            var ourDirector = await _context.OurDirectors.FindAsync(id);
            if (ourDirector == null)
            {
                return NotFound();
            }

            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");

            return View(ourDirector);
        }

        // POST: FossTech/OurDirectors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile Image, OurDirector ourDirector)
        {
            if (id != ourDirector.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var thisMember = await _context.OurDirectors.FirstOrDefaultAsync(i => i.Id == ourDirector.Id);
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

                    thisMember.Name = ourDirector.Name;
                    thisMember.Slug = ourDirector.Slug;
                    thisMember.Description = ourDirector.Description;
                    thisMember.Designation = ourDirector.Designation;
                    thisMember.Education = ourDirector.Education;
                    thisMember.Department = ourDirector.Department;
                    thisMember.SortOrder = ourDirector.SortOrder;
                    thisMember.SectionId  = ourDirector.SectionId;

                    _context.Update(thisMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OurDirectorExists(ourDirector.Id))
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

            return View(ourDirector);
        }

        // GET: FossTech/OurDirectors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OurDirectors == null)
            {
                return NotFound();
            }

            var ourDirector = await _context.OurDirectors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ourDirector == null)
            {
                return NotFound();
            }

            return View(ourDirector);
        }

        // POST: FossTech/OurDirectors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OurDirectors == null)
            {
                return Problem("Entity set 'ApplicationDbContext.OurDirectors'  is null.");
            }
            var ourDirector = await _context.OurDirectors.FindAsync(id);
            if (ourDirector != null)
            {
                _context.OurDirectors.Remove(ourDirector);
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

            var leadsToDelete = await _context.OurDirectors
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.OurDirectors.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool OurDirectorExists(int id)
        {
          return _context.OurDirectors.Any(e => e.Id == id);
        }
    }
}
