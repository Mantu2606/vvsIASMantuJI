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
    public class FranchisesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FranchisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/WebSettings
        public async Task<IActionResult> Index()
        {
            var webSettings = await _context.Franchises.OrderBy(x => x.Id).FirstOrDefaultAsync();
            if (webSettings == null)
            {
                webSettings = new Franchise();
            }
            return View(webSettings);
        }




        // POST: FossTech/WebSettings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Franchise webSetting)
        {

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(webSetting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FranchiseExists(webSetting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)).WithSuccess("Details has been Updated.", null); ;
            }
            return View(webSetting);
        }

        private bool FranchiseExists(int id)
        {
          return _context.Franchises.Any(e => e.Id == id);
        }
    }
}
