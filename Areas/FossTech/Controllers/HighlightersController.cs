using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models;
using FossTech.Extensions;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    public class HighlightersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HighlightersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var highlighter = await _context.Highlighters.OrderBy(x => x.Id).FirstOrDefaultAsync();
            if (highlighter == null)
            {
                highlighter = new Highlighter();
            }
            return View(highlighter);
        }

        // POST: FossTech/Highlighters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Highlighter highlighter)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(highlighter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HighlighterExists(highlighter.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)).WithSuccess("Highlighter has been Updated.", null);
            }
            return View(highlighter);
        }

        
        private bool HighlighterExists(int id)
        {
          return _context.Highlighters.Any(e => e.Id == id);
        }
    }
}
