using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Extensions;
using FossTech.Models;
using Microsoft.AspNetCore.Authorization;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class FooterCodesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FooterCodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var footerCode = await _context.FooterCodes.OrderBy(x => x.Id).FirstOrDefaultAsync();
            return footerCode == null ? View(new FooterCode()) : View(footerCode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(FooterCode footerCode)
        {

            if (ModelState.IsValid)
            {
                if (await _context.BusinessProfile.AnyAsync())
                {
                    try
                    {
                        _context.FooterCodes.Update(footerCode);

                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!FooterCodeExists(footerCode.Id))
                        {
                            return NotFound();
                        }

                        throw;
                    }
                    return RedirectToAction(nameof(Index)).WithSuccess("footer code has been Updated.", null);
                }

                _context.Add(footerCode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)).WithSuccess("footer code has been Updated.", null);
            }

            return View(footerCode).WithError("Please fill all required details.", null);
        }


        private bool FooterCodeExists(int id)
        {
            return _context.FooterCodes.Any(e => e.Id == id);
        }
    }
}
