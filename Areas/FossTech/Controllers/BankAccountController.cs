using FossTech.Data;
using FossTech.Extensions;
using FossTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class BankAccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BankAccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var bankaccount = await _context.BankAccount.OrderBy(x => x.Id).FirstOrDefaultAsync();
            return bankaccount == null ? View(new BankAccount()) : View(bankaccount);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(BankAccount bankaccount)
        {

            if (ModelState.IsValid)
            {
                if (await _context.BankAccount.AnyAsync())
                {
                    try
                    {
                        _context.BankAccount.Update(bankaccount);

                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BusinessProfileExists(bankaccount.Id))
                        {
                            return NotFound();
                        }

                        throw;
                    }
                    return RedirectToAction(nameof(Index)).WithSuccess("Business Profile has been Updated.", null);
                }

                _context.Add(bankaccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)).WithSuccess("Business Profile has been Updated.", null);
            }

            return View(bankaccount).WithError("Please fill all required details.", null);
        }

        private bool BusinessProfileExists(int id)
        {
            return _context.BankAccount.Any(e => e.Id == id);
        }
    }
}

