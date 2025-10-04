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
    public class FranchiseEnquiriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FranchiseEnquiriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/FranchiseEnquiries
        public async Task<IActionResult> Index()
        {
              return View(await _context.FranchiseEnquiries.ToListAsync());
        }

        // GET: FossTech/FranchiseEnquiries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FranchiseEnquiries == null)
            {
                return NotFound();
            }

            var franchiseEnquiry = await _context.FranchiseEnquiries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (franchiseEnquiry == null)
            {
                return NotFound();
            }

            return View(franchiseEnquiry);
        }

        // GET: FossTech/FranchiseEnquiries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FossTech/FranchiseEnquiries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FranchiseEnquiry franchiseEnquiry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(franchiseEnquiry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(franchiseEnquiry);
        }

        // GET: FossTech/FranchiseEnquiries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FranchiseEnquiries == null)
            {
                return NotFound();
            }

            var franchiseEnquiry = await _context.FranchiseEnquiries.FindAsync(id);
            if (franchiseEnquiry == null)
            {
                return NotFound();
            }
            return View(franchiseEnquiry);
        }

        // POST: FossTech/FranchiseEnquiries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FranchiseEnquiry franchiseEnquiry)
        {
            if (id != franchiseEnquiry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(franchiseEnquiry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FranchiseEnquiryExists(franchiseEnquiry.Id))
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
            return View(franchiseEnquiry);
        }

        // GET: FossTech/FranchiseEnquiries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FranchiseEnquiries == null)
            {
                return NotFound();
            }

            var franchiseEnquiry = await _context.FranchiseEnquiries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (franchiseEnquiry == null)
            {
                return NotFound();
            }

            return View(franchiseEnquiry);
        }

        // POST: FossTech/FranchiseEnquiries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FranchiseEnquiries == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FranchiseEnquiries'  is null.");
            }
            var franchiseEnquiry = await _context.FranchiseEnquiries.FindAsync(id);
            if (franchiseEnquiry != null)
            {
                _context.FranchiseEnquiries.Remove(franchiseEnquiry);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.FranchiseEnquiries
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.FranchiseEnquiries.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool FranchiseEnquiryExists(int id)
        {
          return _context.FranchiseEnquiries.Any(e => e.Id == id);
        }
    }
}
