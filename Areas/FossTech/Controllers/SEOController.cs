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

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SEOController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SEOController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var seo = await _context.Seo.OrderBy(x => x.Id).FirstOrDefaultAsync();
            return seo == null ? View(new Seo()) : View(seo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Seo seo)
        {

            if (ModelState.IsValid)
            {
                if (await _context.Seo.AnyAsync())
                {
                    try
                    {
                        _context.Seo.Update(seo);

                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!SeoExists(seo.Id))
                        {
                            return NotFound();
                        }

                        throw;
                    }
                    return RedirectToAction(nameof(Index)).WithSuccess("SEO info has been Updated.", null);
                }

                _context.Add(seo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)).WithSuccess("SEO info has been Updated.", null);
            }

            return View(seo).WithError("Please fill all required details.", null);
        }

       

        private bool SeoExists(int id)
        {
            return _context.Seo.Any(e => e.Id == id);
        }
    }
}
