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

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ImagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ImagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SliderImages
        public async Task<IActionResult> Index()
        {
            return View(await _context.Images
                .OrderBy(x => x.Order)
                .ToListAsync());
        }

        // POST: Admin/SliderImages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(268435456)]
        public async Task<IActionResult> Create(List<IFormFile> images, Image image)
        {
            if (images == null || images.Count <= 0) return RedirectToAction(nameof(Index));

            var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/uploads");

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            foreach (IFormFile item in images)
            {
                // Set Key Name
                var imageName = Guid.NewGuid() + Path.GetExtension(item.FileName);

                // Get path to save
                var savePath = Path.Combine(uploadDirectory, imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await item.CopyToAsync(stream); // Use CopyToAsync to avoid blocking
                }

                // Ensure SectionId and other required fields are set
                var image1 = new Image
                {
                    Img = $"/img/uploads/{imageName}",
                    // Assuming the SectionId is coming from the 'image' parameter
                    SectionId = image.SectionId,
                    // Initialize other properties as needed
                };

                // Add image object to the database context
                _context.Add(image1);
            }

            // Save all changes once after the loop
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)).WithSuccess("Image has been added.", null);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            return View(image);
        }

        // POST: Admin/Logoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(268435456)]
        public async Task<IActionResult> Edit(int id, IFormFile Image,  Image galleryImage)
        {
            if (id != galleryImage.Id)
            {
                return NotFound();
            }
            var thisPost = await _context.Images.FirstAsync(p => p.Id == galleryImage.Id);

            if (thisPost == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid) 
                return View(galleryImage);

            if (Image != null)
            {
                var imageName = Guid.NewGuid() + Path.GetExtension(Image.FileName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/uploads")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/uploads"));
                }

                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/uploads", imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }

                thisPost.Img = $"/img/uploads/{imageName}";
            }
            
            thisPost.Order = galleryImage.Order;
            thisPost.Title = galleryImage.Title;
            thisPost.Category = galleryImage.Category;

            try
            {

                _context.Update(thisPost);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImageExists(galleryImage.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }


        // GET: Admin/Images/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // POST: Admin/SliderImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)).WithSuccess("Image has been deleted.", null);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.Images
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.Images.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool ImageExists(int id)
        {
            return _context.Images.Any(e => e.Id == id);
        }
    }
}