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
    public class FontChangesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FontChangesController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        // GET: FossTech/FontChanges/Edit/5
        public async Task<IActionResult> Index()
        {
            var fontChange = await _context.FontChanges.OrderBy(x => x.Id).FirstOrDefaultAsync();
            if (fontChange == null)
            {
                fontChange = new FontChange();
            }
            return View(fontChange);
        }

        // POST: FossTech/FontChanges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(FontChange fontChange)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fontChange);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FontChangeExists(fontChange.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)).WithSuccess("Font has been Updated.", null);
            }
            return View(fontChange);
        }

        private bool FontChangeExists(int id)
        {
          return _context.FontChanges.Any(e => e.Id == id);
        }
    }
}
