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
    public class WhatsappClickCountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WhatsappClickCountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/WhatsappClickCounts
        public async Task<IActionResult> Index()
        {
              return View(await _context.WhatsappClickCounts.ToListAsync());
        }

        // GET: FossTech/WhatsappClickCounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.WhatsappClickCounts == null)
            {
                return NotFound();
            }

            var whatsappClickCount = await _context.WhatsappClickCounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (whatsappClickCount == null)
            {
                return NotFound();
            }

            return View(whatsappClickCount);
        }

        // GET: FossTech/WhatsappClickCounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FossTech/WhatsappClickCounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreatedAt,LastModified")] WhatsappClickCount whatsappClickCount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(whatsappClickCount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(whatsappClickCount);
        }

        // GET: FossTech/WhatsappClickCounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.WhatsappClickCounts == null)
            {
                return NotFound();
            }

            var whatsappClickCount = await _context.WhatsappClickCounts.FindAsync(id);
            if (whatsappClickCount == null)
            {
                return NotFound();
            }
            return View(whatsappClickCount);
        }

        // POST: FossTech/WhatsappClickCounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CreatedAt,LastModified")] WhatsappClickCount whatsappClickCount)
        {
            if (id != whatsappClickCount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(whatsappClickCount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WhatsappClickCountExists(whatsappClickCount.Id))
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
            return View(whatsappClickCount);
        }

        // GET: FossTech/WhatsappClickCounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.WhatsappClickCounts == null)
            {
                return NotFound();
            }

            var whatsappClickCount = await _context.WhatsappClickCounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (whatsappClickCount == null)
            {
                return NotFound();
            }

            return View(whatsappClickCount);
        }

        // POST: FossTech/WhatsappClickCounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.WhatsappClickCounts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.WhatsappClickCounts'  is null.");
            }
            var whatsappClickCount = await _context.WhatsappClickCounts.FindAsync(id);
            if (whatsappClickCount != null)
            {
                _context.WhatsappClickCounts.Remove(whatsappClickCount);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WhatsappClickCountExists(int id)
        {
          return _context.WhatsappClickCounts.Any(e => e.Id == id);
        }
    }
}
