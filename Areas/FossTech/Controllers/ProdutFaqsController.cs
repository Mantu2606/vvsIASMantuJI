using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models.ProductModels;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    public class ProdutFaqsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProdutFaqsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/ProdutFaqs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProdutFaqs.Include(p => p.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FossTech/ProdutFaqs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produtFaq = await _context.ProdutFaqs
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produtFaq == null)
            {
                return NotFound();
            }

            return View(produtFaq);
        }

        // GET: FossTech/ProdutFaqs/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: FossTech/ProdutFaqs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,Question,Answer,SortOrder")] ProdutFaq produtFaq)
        {
            if (ModelState.IsValid)
            {
                _context.Add(produtFaq);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", produtFaq.ProductId);
            return View(produtFaq);
        }

        // GET: FossTech/ProdutFaqs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produtFaq = await _context.ProdutFaqs.FindAsync(id);
            if (produtFaq == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", produtFaq.ProductId);
            return View(produtFaq);
        }

        // POST: FossTech/ProdutFaqs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Question,Answer,SortOrder")] ProdutFaq produtFaq)
        {
            if (id != produtFaq.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produtFaq);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutFaqExists(produtFaq.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", produtFaq.ProductId);
            return View(produtFaq);
        }

        // GET: FossTech/ProdutFaqs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produtFaq = await _context.ProdutFaqs
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produtFaq == null)
            {
                return NotFound();
            }

            return View(produtFaq);
        }

        // POST: FossTech/ProdutFaqs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produtFaq = await _context.ProdutFaqs.FindAsync(id);
            _context.ProdutFaqs.Remove(produtFaq);
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

            var leadsToDelete = await _context.ProdutFaqs
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.ProdutFaqs.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool ProdutFaqExists(int id)
        {
            return _context.ProdutFaqs.Any(e => e.Id == id);
        }
    }
}
