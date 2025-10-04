using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models.StudyMaterialModels;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    public class StandardsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StandardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/Standards
        public async Task<IActionResult> Index()
        {
            var standard = await _context.Standards.Include(x => x.Board).Include(x => x.Subjects).ToListAsync();
            return View(standard);
        }

        // GET: FossTech/Standards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Standards == null)
            {
                return NotFound();
            }

            var standard = await _context.Standards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (standard == null)
            {
                return NotFound();
            }

            return View(standard);
        }

        // GET: FossTech/Standards/Create
        public async Task<IActionResult> Create()
        {
            var boards =await _context.Boards.ToListAsync();

            if (boards == null || !boards.Any())
            {
                ModelState.AddModelError("", "No boards available. Please add a board first.");
                return View();
            }
            ViewBag.Boards = boards;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Standard standard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(standard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name");
                return View(standard);
        }

        // GET: FossTech/Standards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Standards == null)
            {
                return NotFound();
            }

            var standard = await _context.Standards.Include(x => x.Subjects).FirstOrDefaultAsync(m => m.Id == id);
            if (standard == null)
            {
                return NotFound();
            }
            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name", standard.BoardId);
            return View(standard);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Standard standard)
        {
            if (id != standard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(standard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StandardExists(standard.Id))
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
            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name", standard.BoardId);
            return View(standard);
        }

        // GET: FossTech/Standards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Standards == null)
            {
                return NotFound();
            }

            var standard = await _context.Standards.Include(x => x.Subjects)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (standard == null)
            {
                return NotFound();
            }

            return View(standard);
        }

        // POST: FossTech/Standards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Standards == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Standards'  is null.");
            }
            var standard = await _context.Standards.FindAsync(id);
            if (standard != null)
            {
                _context.Standards.Remove(standard);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StandardExists(int id)
        {
            return _context.Standards.Any(e => e.Id == id);
        }
    }
}
