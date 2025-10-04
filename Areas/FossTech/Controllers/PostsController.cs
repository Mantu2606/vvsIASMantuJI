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
using Fingers10.ExcelExport.ActionResults;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Posts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Posts.ToListAsync());
        }

        public async Task<IActionResult> ExporttoExcel()
        {
            var results = await _context.Posts.ToListAsync();
            return new ExcelResult<Post>(results, "Demo Sheet Name", "Fingers10");
        }

        // GET: Admin/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Admin/Posts/Create
        public IActionResult Create()
        {
            ViewData["Sections"] = new MultiSelectList(_context.Sections, "Id", "Name");
            return View();
        }

        // POST: Admin/Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile formImage, Post post)
        {
            if (ModelState.IsValid)
            {
                if (formImage == null)
                {
                    ModelState.AddModelError("Image", "Image is required.");
                    return View(post);
                }
                var imageName = Guid.NewGuid() + Path.GetExtension(formImage.FileName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Posts")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Posts"));
                }

                //Get url To Save
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Posts", imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    formImage.CopyTo(stream);
                }

                post.Image = $"/img/Posts/{imageName}";
                post.Slug = UrlHelper.GetFriendlyTitle(post.Title);
                post.CreatedAt = DateTime.Now;

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Sections"] = new MultiSelectList(_context.Sections, "Id", "Name");
            return View(post);
        }

        // GET: Admin/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["Sections"] = new MultiSelectList(_context.Sections, "Id", "Name");
            return View(post);
        }

        // POST: Admin/Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  IFormFile image, Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }
            var thisPost = await _context.Posts.FirstAsync(p => p.Id == post.Id);

            if (thisPost == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid) return View(post);

            if (image != null)
            {
                //Set Key Name
                var imageName = Guid.NewGuid() + Path.GetExtension(image.FileName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Posts")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Posts"));
                }

                //Get url To Save
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Posts", imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }

                thisPost.Image = $"/img/Posts/{imageName}";

            }
            try
            {
                thisPost.Slug = UrlHelper.GetFriendlyTitle(post.Title);
                thisPost.Content = post.Content;
                thisPost.Title = post.Title;
                thisPost.SortOrder = post.SortOrder;
                thisPost.Schema = post.Schema;
                thisPost.MetaDescription = post.MetaDescription;
                thisPost.MetaKeywords = post.MetaKeywords;
                thisPost.ImgAlt = post.ImgAlt;
                
                _context.Update(thisPost);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(post.Id))
                {
                    return NotFound();
                }

                throw;
            }
            ViewData["Sections"] = new MultiSelectList(_context.Sections, "Id", "Name");
            return RedirectToAction(nameof(Index));
        }


        // GET: Admin/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
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

            var leadsToDelete = await _context.Posts
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.Posts.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
