using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    public class ProductEnquiriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductEnquiriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/ProductEnquiries
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProductEnquiries.Include(p => p.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FossTech/ProductEnquiries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductEnquiries == null)
            {
                return NotFound();
            }

            var productEnquiry = await _context.ProductEnquiries
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productEnquiry == null)
            {
                return NotFound();
            }

            return View(productEnquiry);
        }

        // GET: FossTech/ProductEnquiries/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: FossTech/ProductEnquiries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductEnquiry productEnquiry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productEnquiry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productEnquiry.ProductId);
            return View(productEnquiry);
        }

        // GET: FossTech/ProductEnquiries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProductEnquiries == null)
            {
                return NotFound();
            }

            var productEnquiry = await _context.ProductEnquiries.FindAsync(id);
            if (productEnquiry == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productEnquiry.ProductId);
            return View(productEnquiry);
        }

        // POST: FossTech/ProductEnquiries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEnquiry productEnquiry)
        {
            if (id != productEnquiry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productEnquiry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductEnquiryExists(productEnquiry.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productEnquiry.ProductId);
            return View(productEnquiry);
        }

        // GET: FossTech/ProductEnquiries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductEnquiries == null)
            {
                return NotFound();
            }

            var productEnquiry = await _context.ProductEnquiries
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productEnquiry == null)
            {
                return NotFound();
            }

            return View(productEnquiry);
        }

        // POST: FossTech/ProductEnquiries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductEnquiries == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ProductEnquiries'  is null.");
            }
            var productEnquiry = await _context.ProductEnquiries.FindAsync(id);
            if (productEnquiry != null)
            {
                _context.ProductEnquiries.Remove(productEnquiry);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.ProductEnquiries
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.ProductEnquiries.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool ProductEnquiryExists(int id)
        {
          return _context.ProductEnquiries.Any(e => e.Id == id);
        }
    }
}
