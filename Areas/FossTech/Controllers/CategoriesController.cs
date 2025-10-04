using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models;
using Fingers10.ExcelExport.ActionResults;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        public async Task<IActionResult> ExporttoExcel()
        {
            var results = await _context.Categories.ToListAsync();
            return new ExcelResult<Category>(results, "Demo Sheet Name", "Fingers10");
        }

        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.OrderBy(x => x.Id)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            ViewData["Sections"] = new MultiSelectList(_context.Sections, "Id", "Name");
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile formImage, Category category)
        {
            ViewData["Sections"] = new MultiSelectList(_context.Sections, "Id", "Name");

            if (ModelState.IsValid)
            {
                if (formImage == null)
                {
                    ModelState.AddModelError("Image", "Image is required.");
                    return View(category);
                }

                var imageName = Guid.NewGuid() + Path.GetExtension(formImage.FileName);

                // Ensure the directory exists
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Category");
                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }

                // Get URL to save
                var savePath = Path.Combine(imagePath, imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await formImage.CopyToAsync(stream);
                }

                category.Image = $"/img/Category/{imageName}";

                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        //public async Task<IActionResult>Create(IFormFile formImage, Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (formImage == null)
        //        {
        //            ModelState.AddModelError("Image", "Image is required.");
        //            return View(category);
        //        }
        //        var imageName = Guid.NewGuid() + Path.GetExtension(formImage.FileName);

        //        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Category")))
        //        {
        //            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Category"));
        //        }

        //        //Get url To Save
        //        var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Category", imageName);

        //        await using (var stream = new FileStream(savePath, FileMode.Create))
        //        {
        //            formImage.CopyTo(stream);
        //        }

        //        category.Image = $"/img/Category/{imageName}";


        //        _context.Add(category);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //}

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["Sections"] = new MultiSelectList(_context.Sections, "Id", "Name");
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile image, Category category,Section Sections)
        {
            ViewData["Sections"] = new MultiSelectList(_context.Sections, "Id", "Name");
            if (id != category.Id)
            {
                return NotFound();
            }
            var thisPost = await _context.Categories.FirstAsync(p => p.Id == category.Id);

            if (thisPost == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid) return View(category);

            if (image != null)
            {
                //Set Key Name
                var imageName = Guid.NewGuid() + Path.GetExtension(image.FileName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Category")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Category"));
                }

                //Get url To Save
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Category", imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }

                thisPost.Image = $"/img/Category/{imageName}";

            }
            try
            {

                thisPost.Name = category.Name;
                thisPost.Featured = category.Featured;
                thisPost.Status = category.Status;
                thisPost.SortOrder = category.SortOrder;
               thisPost.SectionId= category.SectionId;

                _context.Update(thisPost);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
                {
                    return NotFound();
                }

                throw;
            }
           
            return RedirectToAction(nameof(Index));

        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.OrderBy(x => x.Id)
                .FirstOrDefaultAsync(m => m.Id == id);


            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            var products = await _context.ProductCategories.Where(x => x.CategoryId == category.Id).ToListAsync();

            _context.RemoveRange(products);
            await _context.SaveChangesAsync();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.Categories
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.Categories.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
