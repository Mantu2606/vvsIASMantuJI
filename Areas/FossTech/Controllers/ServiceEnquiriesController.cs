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
    public class ServiceEnquiriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceEnquiriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/ServiceEnquiries
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ServiceEnquiries.Include(s => s.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FossTech/ServiceEnquiries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ServiceEnquiries == null)
            {
                return NotFound();
            }

            var serviceEnquiry = await _context.ServiceEnquiries
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceEnquiry == null)
            {
                return NotFound();
            }

            return View(serviceEnquiry);
        }

        // GET: FossTech/ServiceEnquiries/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: FossTech/ServiceEnquiries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceEnquiry serviceEnquiry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviceEnquiry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", serviceEnquiry.ProductId);
            return View(serviceEnquiry);
        }

        // GET: FossTech/ServiceEnquiries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ServiceEnquiries == null)
            {
                return NotFound();
            }

            var serviceEnquiry = await _context.ServiceEnquiries.FindAsync(id);
            if (serviceEnquiry == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", serviceEnquiry.ProductId);
            return View(serviceEnquiry);
        }

        // POST: FossTech/ServiceEnquiries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceEnquiry serviceEnquiry)
        {
            if (id != serviceEnquiry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceEnquiry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceEnquiryExists(serviceEnquiry.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", serviceEnquiry.ProductId);
            return View(serviceEnquiry);
        }

        // GET: FossTech/ServiceEnquiries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ServiceEnquiries == null)
            {
                return NotFound();
            }

            var serviceEnquiry = await _context.ServiceEnquiries
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceEnquiry == null)
            {
                return NotFound();
            }

            return View(serviceEnquiry);
        }

        // POST: FossTech/ServiceEnquiries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ServiceEnquiries == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ServiceEnquiries'  is null.");
            }
            var serviceEnquiry = await _context.ServiceEnquiries.FindAsync(id);
            if (serviceEnquiry != null)
            {
                _context.ServiceEnquiries.Remove(serviceEnquiry);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.ServiceEnquiries
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.ServiceEnquiries.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool ServiceEnquiryExists(int id)
        {
          return _context.ServiceEnquiries.Any(e => e.Id == id);
        }
    }
}
