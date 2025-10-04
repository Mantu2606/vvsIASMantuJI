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
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ColorCombinationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ColorCombinationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/ColorCombinations
        public async Task<IActionResult> Index()
        {
            var colorCombination = await _context.ColorCombinations.OrderBy(x => x.Id).FirstOrDefaultAsync();
            if (colorCombination == null)
            {
                colorCombination = new ColorCombination();
            }
            return View(colorCombination);
        }

        // POST: FossTech/ColorCombinations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ColorCombination colorCombination)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(colorCombination);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ColorCombinationExists(colorCombination.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)).WithSuccess("Color Combination has been Updated.", null); 
            }
            return View(colorCombination);
        }

        private bool ColorCombinationExists(int id)
        {
          return (_context.ColorCombinations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
