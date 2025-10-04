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
    public class OurToppersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OurToppersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/OurToppers
        public async Task<IActionResult> Index()
        {
            return View(await _context.OurToppers
                .Include(b => b.Section)
                .ToListAsync());
        }

        // GET: FossTech/OurToppers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OurToppers == null)
            {
                return NotFound();
            }

            var ourTopper = await _context.OurToppers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ourTopper == null)
            {
                return NotFound();
            }

            return View(ourTopper);
        }

        // GET: FossTech/OurToppers/Create
        public IActionResult Create()
        {
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");
            ViewData["Year"] = new SelectList(Enumerable.Range(2019, DateTime.Now.Year - 2019 + 1).Select(t => t.ToString()));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile formImage, OurTopper post)
        {
            if (ModelState.IsValid)
            {
                if (formImage != null)
                {
                    var imageName = Guid.NewGuid() + Path.GetExtension(formImage.FileName);

                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Posts")))
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Posts"));
                    }
                    var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Posts", imageName);

                    await using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        formImage.CopyTo(stream);
                    }

                    post.Image = $"/img/Posts/{imageName}";
                }
                post.CreatedAt = DateTime.Now;

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");
            ViewData["TopperYear"] = new SelectList(Enumerable.Range(2019, DateTime.Now.Year - 2019 + 1).Select(t => t.ToString()));
            return View(post);
        }

        // GET: FossTech/OurToppers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OurToppers == null)
            {
                return NotFound();
            }

            var ourTopper = await _context.OurToppers.FindAsync(id);
            if (ourTopper == null)
            {
                return NotFound();
            }
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");
            ViewData["TopperYear"] = new SelectList(Enumerable.Range(2019, DateTime.Now.Year - 2019 + 1).Select(t => t.ToString()));
            return View(ourTopper);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile image, OurTopper post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }
            var thisPost = await _context.OurToppers.FirstAsync(p => p.Id == post.Id);

            if (thisPost == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");
                ViewData["TopperYear"]=new SelectList(Enumerable.Range(2019, DateTime.Now.Year - 2019 + 1).Select(t => t.ToString()));

                return View(post);
            }
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

                thisPost.Name = post.Name;
                thisPost.Slug = post.Slug;
                thisPost.SortOrder = post.SortOrder;
                thisPost.SchoolName = post.SchoolName;
                thisPost.Percentage = post.Percentage;
                thisPost.Description = post.Description;
                thisPost.SectionId = post.SectionId;
                thisPost.TopperYear = post.TopperYear;
                thisPost.ShowStar = post.ShowStar;
                _context.Update(thisPost);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OurTopperExists(post.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: FossTech/OurToppers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OurToppers == null)
            {
                return NotFound();
            }

            var ourTopper = await _context.OurToppers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ourTopper == null)
            {
                return NotFound();
            }
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");

            return View(ourTopper);
        }

        // POST: FossTech/OurToppers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OurToppers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.OurToppers'  is null.");
            }
            var ourTopper = await _context.OurToppers.FindAsync(id);
            if (ourTopper != null)
            {
                _context.OurToppers.Remove(ourTopper);
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

            var leadsToDelete = await _context.OurToppers
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.OurToppers.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool OurTopperExists(int id)
        {
            return _context.OurToppers.Any(e => e.Id == id);
        }
    }
}
