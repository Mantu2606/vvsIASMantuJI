using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fingers10.ExcelExport.ActionResults;
using FossTech.Data;
using FossTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class CollectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CollectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/Collections
        public async Task<IActionResult> Index()
        {
            return View(await _context.Collections.ToListAsync());
        }

        public async Task<IActionResult> ExporttoExcel()
        {
            var results = await _context.Collections.ToListAsync();
            return new ExcelResult<Collection>(results, "Collection Details", "Collection");
        }

        // GET: FossTech/Collections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // GET: FossTech/Collections/Create
        public IActionResult Create()
        {
            ViewData["Products"] = new SelectList(_context.Products, "Id", "Name");

            return View();
        }

        // POST: FossTech/Collections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile CollectionImage, List<int> products, Collection collection)
        {
            if (ModelState.IsValid)
            {
                if (CollectionImage == null)
                {
                    ModelState.AddModelError("Image", "Collection Image is required.");
                    ViewData["Products"] = new SelectList(_context.Products, "Id", "Name");
                    return View(collection);
                }

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/collections")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot/img/collections"));
                }
                var mainImageName = Guid.NewGuid() + Path.GetExtension(CollectionImage.FileName);

                //Get url To Save
                var mainImageSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/collections", mainImageName);

                await using (var stream = new FileStream(mainImageSavePath, FileMode.Create))
                {
                    await CollectionImage.CopyToAsync(stream);
                }

                collection.Image = $"/img/collections/{mainImageName}";

                _context.Add(collection);
                await _context.SaveChangesAsync();

                if (products != null)
                {
                    foreach (var product in products)
                    {
                        await _context.AddAsync(entity: new CollectionProduct
                        {
                            ProductId = product,
                            CollectionId = collection.Id
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["Products"] = new SelectList(_context.Products, "Id", "Name");
            return View(collection);
        }

        // GET: FossTech/Collections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections.FindAsync(id);

            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name",
               _context.CollectionProducts.Where(a => a.CollectionId == id).Select(e => e.ProductId).ToArray());

            if (collection == null)
            {
                return NotFound();
            }
            return View(collection);
        }

        // POST: FossTech/Collections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile CollectionImage, List<int> products, Collection collection)
        {
            if (id != collection.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldCollection = await _context.Collections.FindAsync(id);

                    if (CollectionImage != null)
                    {
                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/collections")))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                                "wwwroot/img/collections"));
                        }
                        var mainImageName = Guid.NewGuid() + Path.GetExtension(CollectionImage.FileName);

                        //Get url To Save
                        var mainImageSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/collections", mainImageName);

                        await using (var stream = new FileStream(mainImageSavePath, FileMode.Create))
                        {
                            await CollectionImage.CopyToAsync(stream);
                        }

                        oldCollection.Image = $"/img/collections/{mainImageName}";
                    }
                    oldCollection.Slug = collection.Slug;
                    oldCollection.Name = collection.Name;

                    var collectionProducts = await _context.CollectionProducts.Where(x => x.CollectionId == id).ToListAsync();

                    _context.RemoveRange(collectionProducts);
                    await _context.SaveChangesAsync();

                    if (products != null)
                    {
                        foreach (var product in products)
                        {
                            await _context.AddAsync(entity: new CollectionProduct
                            {
                                ProductId = product,
                                CollectionId = collection.Id
                            });
                        }
                    }


                    _context.Update(oldCollection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollectionExists(collection.Id))
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
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name",
             _context.CollectionProducts.Where(a => a.CollectionId == id).Select(e => e.ProductId).ToArray());
            return View(collection);
        }

        // GET: FossTech/Collections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // POST: FossTech/Collections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var collection = await _context.Collections.FindAsync(id);
            _context.Collections.Remove(collection);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CollectionExists(int id)
        {
            return _context.Collections.Any(e => e.Id == id);
        }
    }
}
