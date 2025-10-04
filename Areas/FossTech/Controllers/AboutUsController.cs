using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Helpers;
using FossTech.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AboutUsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AboutUsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/AboutUs
        public async Task<IActionResult> Index()
        {
            return View(await _context.AboutUs
                .Include(x => x.Section)
                .ToListAsync());
        }

        // GET: Admin/AboutUs/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var aboutUs = await _context.AboutUs
        //        .FirstOrDefaultAsync(m => m.AboutUsId == id);
        //    if (aboutUs == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(aboutUs);
        //}

        // GET: Admin/AboutUs/Create
        public IActionResult Create()
        {
            ViewData["SectionId"] = new MultiSelectList(_context.Sections, "Id", "Name");
            return View();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logo = await _context.AboutUs.FindAsync(id);
            if (logo == null)
            {
                return NotFound();
            }
            ViewData["SectionId"] = new MultiSelectList(_context.Sections, "Id", "Name");

            return View(logo);
        }
        // POST: Admin/AboutUs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(IFormFile formImage, AboutUs aboutUs)
        {
            if (ModelState.IsValid)
            {
                if (formImage == null)
                {
                    ModelState.AddModelError("Image", "Image is required.");
                    return View(aboutUs);
                }
                var imageName = Guid.NewGuid() + Path.GetExtension(formImage.FileName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Abouts")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Abouts"));
                }

                //Get url To Save
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Abouts", imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    formImage.CopyTo(stream);
                }

                aboutUs.Image = $"/img/Abouts/{imageName}";
                aboutUs.Slug = UrlHelper.GetFriendlyTitle(aboutUs.Title);

                _context.Add(aboutUs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["SectionId"] = new MultiSelectList(_context.Sections, "Id", "Name");

            return View(aboutUs);
        }

        // GET: Admin/AboutUs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile image, AboutUs post, Section Sections)
        {
            if (id != post.AboutUsId)
            {
                return NotFound();
            }
            var thisPost = await _context.AboutUs.FirstAsync(p => p.AboutUsId == post.AboutUsId);

            if (thisPost == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewData["SectionId"] = new MultiSelectList(_context.Sections, "Id", "Name");

                return View(post);
            }
            if (image != null)
            {
                //Set Key Name
                var imageName = Guid.NewGuid() + Path.GetExtension(image.FileName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Abouts")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Abouts"));
                }

                //Get url To Save
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Abouts", imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }

                thisPost.Image = $"/img/Abouts/{imageName}";

            }

            try
            {
               
                thisPost.Description = post.Description;
                thisPost.Title = post.Title;
                thisPost.Keywords = post.Keywords;
                thisPost.Slug = post.Slug;
                thisPost.Video = post.Video;
                thisPost.SectionId = post.SectionId;
                _context.Update(thisPost);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AboutUsExists(post.AboutUsId))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/AboutUs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        

        // GET: Admin/AboutUs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aboutUs = await _context.AboutUs.OrderBy(x => x.AboutUsId).OrderBy(x=>x.SortOrder)
                .FirstOrDefaultAsync(m => m.AboutUsId == id);
            if (aboutUs == null)
            {
                return NotFound();
            }

            return View(aboutUs);
        }

        // POST: Admin/AboutUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aboutUs = await _context.AboutUs.FindAsync(id);
            _context.AboutUs.Remove(aboutUs);
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

            var leadsToDelete = await _context.AboutUs
            .Where(lead => leadIds.Contains(lead.AboutUsId))
            .ToListAsync();

            _context.AboutUs.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool AboutUsExists(int id)
        {
            return _context.AboutUs.Any(e => e.AboutUsId == id);
        }
    }
}
