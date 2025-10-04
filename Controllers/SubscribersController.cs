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

namespace FossTech.Controllers
{
    public class SubscribersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubscribersController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsSubscriber newsSubscriber)
        {
            if (ModelState.IsValid)
            {
                var sub = await _context.NewsSubscribers.FirstOrDefaultAsync(x => x.Email == newsSubscriber.Email);
                if (sub != null)
                {
                    return RedirectToAction("Index", "Home").WithWarning("Already subscribed", "Already subscribed!");
                }
                _context.Add(newsSubscriber);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home").WithSuccess("Successful", "Successfully subscribed");
            }
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return RedirectToAction("Index", "Home").WithError("Error", "Something Wrong Please try again after some time.");
        }

    }
}
