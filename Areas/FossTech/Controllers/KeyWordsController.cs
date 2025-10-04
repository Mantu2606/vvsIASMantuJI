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
    public class KeyWordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KeyWordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/KeyWords
        public async Task<IActionResult> Index()
        {
              return View(await _context.KeyWords.ToListAsync());
        }

        // GET: FossTech/KeyWords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.KeyWords == null)
            {
                return NotFound();
            }

            var keyWord = await _context.KeyWords
                .FirstOrDefaultAsync(m => m.Id == id);
            if (keyWord == null)
            {
                return NotFound();
            }

            return View(keyWord);
        }

        // GET: FossTech/KeyWords/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FossTech/KeyWords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KeyWord keyWord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(keyWord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(keyWord);
        }

        // GET: FossTech/KeyWords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.KeyWords == null)
            {
                return NotFound();
            }

            var keyWord = await _context.KeyWords.FindAsync(id);
            if (keyWord == null)
            {
                return NotFound();
            }
            return View(keyWord);
        }

        // POST: FossTech/KeyWords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KeyWord keyWord)
        {
            if (id != keyWord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(keyWord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KeyWordExists(keyWord.Id))
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
            return View(keyWord);
        }

        // GET: FossTech/KeyWords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.KeyWords == null)
            {
                return NotFound();
            }

            var keyWord = await _context.KeyWords
                .FirstOrDefaultAsync(m => m.Id == id);
            if (keyWord == null)
            {
                return NotFound();
            }

            return View(keyWord);
        }

        // POST: FossTech/KeyWords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.KeyWords == null)
            {
                return Problem("Entity set 'ApplicationDbContext.KeyWords'  is null.");
            }
            var keyWord = await _context.KeyWords.FindAsync(id);
            if (keyWord != null)
            {
                _context.KeyWords.Remove(keyWord);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.KeyWords
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.KeyWords.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool KeyWordExists(int id)
        {
          return _context.KeyWords.Any(e => e.Id == id);
        }
    }
}
