using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models.TestPaperModels;
using FossTech.Models.StudyMaterialModels;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    public class TestPapersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestPapersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/TestPapers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TestPapers.Include(t => t.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FossTech/TestPapers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TestPapers == null)
            {
                return NotFound();
            }

            var testPaper = await _context.TestPapers
                .Include(t => t.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testPaper == null)
            {
                return NotFound();
            }

            return View(testPaper);
        }

        // GET: FossTech/TestPapers/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: FossTech/TestPapers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<IFormFile> PDFs, IFormFile formImage, TestPaper testPaper)
        {
            if (ModelState.IsValid)
            {
                if (formImage == null)
                {
                    ModelState.AddModelError("Thumbnail Image", "Image is required.");
                    return View(testPaper);
                }
                var imageNamee = Guid.NewGuid() + Path.GetExtension(formImage.FileName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/testPaper")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/testPaper"));
                }

                //Get url To Save
                var savePathh = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/testPaper", imageNamee);

                await using (var stream = new FileStream(savePathh, FileMode.Create))
                {
                    formImage.CopyTo(stream);
                }

                testPaper.ThumbnailImage = $"/img/testPaper/{imageNamee}";

                testPaper.CreatedAt = DateTime.Now;
                testPaper.LastModified = DateTime.Now;
                _context.Add(testPaper);
                await _context.SaveChangesAsync();

                if (PDFs != null)
                {
                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf/testPaper")))
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                            "wwwroot/pdf/testPaper"));
                    }

                    foreach (IFormFile item in PDFs)
                    {
                        //Set Key Name
                        var imageName = Guid.NewGuid() + Path.GetExtension(item.FileName);

                        //Get url To Save
                        var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf/testPaper", imageName);

                        await using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await item.CopyToAsync(stream);
                        }

                        var pdf = new TestPaperPDF()
                        {
                            TestPaperId = testPaper.Id,
                            PDF = $"/pdf/testPaper/{imageName}"
                        };

                        _context.Add(pdf);
                    }

                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", testPaper.ProductId);
            return View(testPaper);
        }

        // GET: FossTech/TestPapers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TestPapers == null)
            {
                return NotFound();
            }

            var testPaper = await _context.TestPapers.Include(x => x.PDFs).FirstOrDefaultAsync(x => x.Id == id);
            if (testPaper == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", testPaper.ProductId);
            return View(testPaper);
        }

        // POST: FossTech/TestPapers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, List<IFormFile> PDFs, IFormFile image, TestPaper testPaper)
        {
            if (id != testPaper.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (image != null)
                    {
                        //Set Key Name
                        var imageNamee = Guid.NewGuid() + Path.GetExtension(image.FileName);

                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/testPaper")))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/testPaper"));
                        }

                        //Get url To Save
                        var savePathh = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/testPaper", imageNamee);

                        await using (var stream = new FileStream(savePathh, FileMode.Create))
                        {
                            image.CopyTo(stream);
                        }

                        testPaper.ThumbnailImage = $"/img/testPaper/{imageNamee}";

                    }
                    if (PDFs != null)
                    {
                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf/testPaper")))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                                "wwwroot/pdf/testPaper"));
                        }

                        foreach (IFormFile item in PDFs)
                        {
                            //Set Key Name
                            var imageName = Guid.NewGuid() + Path.GetExtension(item.FileName);

                            //Get url To Save
                            var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf/testPaper", imageName);

                            await using (var stream = new FileStream(savePath, FileMode.Create))
                            {
                                await item.CopyToAsync(stream);
                            }

                            var pdf = new TestPaperPDF()
                            {
                                TestPaperId = testPaper.Id,
                                PDF = $"/pdf/testPaper/{imageName}"
                            };

                            _context.Add(pdf);
                        }

                        await _context.SaveChangesAsync();
                    }

                    _context.Update(testPaper);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestPaperExists(testPaper.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", testPaper.ProductId);
            return View(testPaper);
        }

        // GET: FossTech/TestPapers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TestPapers == null)
            {
                return NotFound();
            }

            var testPaper = await _context.TestPapers
                .Include(t => t.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testPaper == null)
            {
                return NotFound();
            }

            return View(testPaper);
        }

        // POST: FossTech/TestPapers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TestPapers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TestPapers'  is null.");
            }
            var testPaper = await _context.TestPapers.FindAsync(id);
            if (testPaper != null)
            {
                _context.TestPapers.Remove(testPaper);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestPaperExists(int id)
        {
          return _context.TestPapers.Any(e => e.Id == id);
        }

        public async Task<IActionResult> DeleteTestPaperPdf(int pdfId, int testPaperId)
        {
            var testPaperPdf = await _context.TestPaperPDFs.FindAsync(pdfId);
            _context.TestPaperPDFs.Remove(testPaperPdf);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = testPaperId });
        }

    }
}
