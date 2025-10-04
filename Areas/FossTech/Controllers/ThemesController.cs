using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ThemesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public ThemesController(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        // GET: FossTech/Themes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Themes.OrderBy(x => x.SortOrder).ToListAsync());
        }
        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var theTheme = await _context.Themes.FindAsync(id);
            if (theTheme == null)
            {
                return NotFound();
            }

            var themes = await _context.Themes.ToListAsync();

            // Set all themes' status to false
            foreach (var theme in themes)
            {
                theme.Status = false;
            }

            // Find the theme with the specified id and set its status to true
            var selectedTheme = themes.FirstOrDefault(t => t.Id == id);
            if (selectedTheme != null)
            {
                selectedTheme.Status = true;

                // Remove the cached theme
                _memoryCache.Remove("CurrentTheme");
                HttpContext.Items.Remove("CurrentTheme");


                Response.Cookies.Append("theme", selectedTheme.Name, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true
                });
            }

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/FAQs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Themes.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }
            return View(page);
        }

        // POST: Admin/FAQs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Theme theme)
        {
            if (id != theme.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(theme);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PageExists(theme.Id))
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
            return View(theme);
        }

        private bool PageExists(int id)
        {
            return _context.Themes.Any(e => e.Id == id);
        }
    }
}
