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
    public class PlacementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlacementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/Placements
        public async Task<IActionResult> Index()
        {
            return View(await _context.Placements
                .OrderBy(x => x.Order)
                .ToListAsync());
        }

        // GET: FossTech/Placements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placement = await _context.Placements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (placement == null)
            {
                return NotFound();
            }

            return View(placement);
        }

        // GET: FossTech/Placements/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FossTech/Placements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile Image, Placement placement)
        {
            if (Image == null)
            {
                ModelState.AddModelError("Image", "Image is required.");
                return View(placement);
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

            placement.Image = $"/img/testimonial/{mainImageName}";

            if (ModelState.IsValid)
            {
                _context.Add(placement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(placement);
        }

        // GET: FossTech/Placements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placement = await _context.Placements.FindAsync(id);
            if (placement == null)
            {
                return NotFound();
            }
            return View(placement);
        }

        // POST: FossTech/Placements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile Image, Placement placement)
        {
            if (id != placement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var thisMember = await _context.Placements.FirstOrDefaultAsync(i => i.Id == placement.Id);
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
                    thisMember.Name = placement.Name;
                    thisMember.CompanyName = placement.CompanyName;
                    thisMember.Order = placement.Order;

                    _context.Update(placement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlacementExists(placement.Id))
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
            return View(placement);
        }

        // GET: FossTech/Placements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placement = await _context.Placements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (placement == null)
            {
                return NotFound();
            }

            return View(placement);
        }

        // POST: FossTech/Placements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var placement = await _context.Placements.FindAsync(id);
            _context.Placements.Remove(placement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.Placements
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.Placements.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool PlacementExists(int id)
        {
            return _context.Placements.Any(e => e.Id == id);
        }
    }
}