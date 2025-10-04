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
    public class BotAttemptsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BotAttemptsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var bot=await _context.BotAttempts.ToListAsync();
            if (User.IsInRole("Admin"))
            {
                ViewData["ShowModal"] = true;
            }
            else
            {
                ViewData["ShowModal"] = false;
            }
            return View(bot);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BotAttempts == null)
            {
                return NotFound();
            }

            var botAttempt = await _context.BotAttempts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (botAttempt == null)
            {
                return NotFound();
            }

            return View(botAttempt);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BotAttempt botAttempt)
        {
            if (ModelState.IsValid)
            {
                _context.Add(botAttempt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(botAttempt);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BotAttempts == null)
            {
                return NotFound();
            }

            var botAttempt = await _context.BotAttempts.FindAsync(id);
            if (botAttempt == null)
            {
                return NotFound();
            }
            return View(botAttempt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BotAttempt botAttempt)
        {
            if (id != botAttempt.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(botAttempt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BotAttemptExists(botAttempt.Id))
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
            return View(botAttempt);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BotAttempts == null)
            {
                return NotFound();
            }

            var botAttempt = await _context.BotAttempts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (botAttempt == null)
            {
                return NotFound();
            }

            return View(botAttempt);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BotAttempts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BotAttempts'  is null.");
            }
            var botAttempt = await _context.BotAttempts.FindAsync(id);
            if (botAttempt != null)
            {
                _context.BotAttempts.Remove(botAttempt);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BotAttemptExists(int id)
        {
            return _context.BotAttempts.Any(e => e.Id == id);
        }
    }
}
