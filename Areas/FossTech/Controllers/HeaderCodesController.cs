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
    public class HeaderCodesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HeaderCodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var headerCode = await _context.HeaderCodes.OrderBy(x => x.Id).FirstOrDefaultAsync();
            return headerCode == null ? View(new HeaderCode()) : View(headerCode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(HeaderCode headerCode)
        {

            if (ModelState.IsValid)
            {
                if (await _context.BusinessProfile.AnyAsync())
                {
                    try
                    {
                        _context.HeaderCodes.Update(headerCode);

                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!HeaderCodeExists(headerCode.Id))
                        {
                            return NotFound();
                        }

                        throw;
                    }
                    return RedirectToAction(nameof(Index)).WithSuccess("Header code has been Updated.", null);
                }

                _context.Add(headerCode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)).WithSuccess("Header code has been Updated.", null);
            }

            return View(headerCode).WithError("Please fill all required details.", null);
        }

       
        private bool HeaderCodeExists(int id)
        {
            return _context.HeaderCodes.Any(e => e.Id == id);
        }
    }
}
