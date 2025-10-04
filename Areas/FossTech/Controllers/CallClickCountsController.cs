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
    public class CallClickCountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CallClickCountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/CallClickCounts
        public async Task<IActionResult> Index()
        {
              return View(await _context.CallClickCounts.ToListAsync());
        }

        // GET: FossTech/CallClickCounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CallClickCounts == null)
            {
                return NotFound();
            }

            var callClickCount = await _context.CallClickCounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (callClickCount == null)
            {
                return NotFound();
            }

            return View(callClickCount);
        }

        // GET: FossTech/CallClickCounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FossTech/CallClickCounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreatedAt,LastModified")] CallClickCount callClickCount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(callClickCount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(callClickCount);
        }

        // GET: FossTech/CallClickCounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CallClickCounts == null)
            {
                return NotFound();
            }

            var callClickCount = await _context.CallClickCounts.FindAsync(id);
            if (callClickCount == null)
            {
                return NotFound();
            }
            return View(callClickCount);
        }

        // POST: FossTech/CallClickCounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CreatedAt,LastModified")] CallClickCount callClickCount)
        {
            if (id != callClickCount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(callClickCount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CallClickCountExists(callClickCount.Id))
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
            return View(callClickCount);
        }

        // GET: FossTech/CallClickCounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CallClickCounts == null)
            {
                return NotFound();
            }

            var callClickCount = await _context.CallClickCounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (callClickCount == null)
            {
                return NotFound();
            }

            return View(callClickCount);
        }

        // POST: FossTech/CallClickCounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CallClickCounts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CallClickCounts'  is null.");
            }
            var callClickCount = await _context.CallClickCounts.FindAsync(id);
            if (callClickCount != null)
            {
                _context.CallClickCounts.Remove(callClickCount);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CallClickCountExists(int id)
        {
          return _context.CallClickCounts.Any(e => e.Id == id);
        }
    }
}
