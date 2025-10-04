using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Extensions;
using FossTech.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class SliderImagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SliderImagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SliderImages
        public async Task<IActionResult> Index()
        {
            return View(await _context.SliderImages
                .Include(b => b.Section)
                .OrderBy(x => x.Order)
                .ToListAsync());
        }

        // POST: Admin/SliderImages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(268435456)]
        public async Task<IActionResult> Create(List<IFormFile> images)
        {
             ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");

            if (images == null || images.Count <= 0) return RedirectToAction(nameof(Index));

            // Ensure directory exists
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/slider");
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            foreach (IFormFile item in images)
            {
                // Set unique image name
                var imageName = Guid.NewGuid() + Path.GetExtension(item.FileName);
                var savePath = Path.Combine(imagePath, imageName);

                // Save image
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await item.CopyToAsync(stream);
                }

                // Save image information to database
                var image = new SliderImage { Image = $"/img/slider/{imageName}" };
                _context.Add(image);
                await _context.SaveChangesAsync();
            }

            // Redirect to Index with success message
            return RedirectToAction(nameof(Index)).WithSuccess("Slider image has been added.", null);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sliderImage = await _context.SliderImages.FindAsync(id);
            if (sliderImage == null)
            {
                return NotFound();
            }
            ViewData["Sections"] = new MultiSelectList(_context.Sections, "Id", "Name");
            return View(sliderImage);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile img,IFormFile mobileimg,  SliderImage sliderImage)
        {

            var oldSliderImage = await _context.SliderImages.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (img != null)
            {
                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/slider")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot/img/slider"));
                }

                var imageName = Guid.NewGuid() + Path.GetExtension(img.FileName);

                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/slider", imageName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    img.CopyTo(stream);
                }

                oldSliderImage.Image = $"/img/slider/{imageName}";
            }           
   

            if (mobileimg != null)
            {
                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/slider")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot/img/slider"));
                }

                var imageName = Guid.NewGuid() + Path.GetExtension(mobileimg.FileName);

                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/slider", imageName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    mobileimg.CopyTo(stream);
                }

                oldSliderImage.MobileImage = $"/img/slider/{imageName}";
            }

            oldSliderImage.Order = sliderImage.Order;
            oldSliderImage.SectionId = sliderImage.SectionId;   

            _context.Update(oldSliderImage);

            await _context.SaveChangesAsync();

            
            return RedirectToAction(nameof(Index)).WithSuccess("Slider image has been updated.", null);
        }

        // GET: Admin/SliderImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sliderImage = await _context.SliderImages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sliderImage == null)
            {
                return NotFound();
            }
            
            return View(sliderImage);
        }

        // POST: Admin/SliderImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sliderImage = await _context.SliderImages.FindAsync(id);
            _context.SliderImages.Remove(sliderImage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)).WithSuccess("Slider image has been deleted.", null);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.SliderImages
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.SliderImages.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool SliderImageExists(int id)
        {
            return _context.SliderImages.Any(e => e.Id == id);
        }
    }
}
