using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Extensions;
using FossTech.Models;
using Fingers10.ExcelExport.ActionResults;
using System.Collections.Generic;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ContactMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactMessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ContactMessages
        public async Task<IActionResult> Index()
        {
            return View(await _context.ContactMessages.OrderByDescending(x => x.Id).ToListAsync());
        }

        public async Task<IActionResult> ExporttoExcel()
        {
            var results = await _context.ContactMessages.ToListAsync();
            return new ExcelResult<ContactMessage>(results, "Demo Sheet Name", "Fingers10");


        }


        // GET: Admin/ContactMessages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactMessage = await _context.ContactMessages.OrderBy(x => x.Id)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactMessage == null)
            {
                return NotFound();
            }

            return View(contactMessage);
        }

        // GET: Admin/ContactMessages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ContactMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactMessage contactMessage)
        {
            if (ModelState.IsValid)
            {
                contactMessage.CreatedAt = DateTime.Now;
                _context.Add(contactMessage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)).WithSuccess("Message has been added.", null);
            }
            return View(contactMessage).WithError("Please fill all required details.", null);
        }

        // GET: Admin/ContactMessages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactMessage = await _context.ContactMessages.FindAsync(id);
            if (contactMessage == null)
            {
                return NotFound();
            }
            return View(contactMessage);
        }

        // POST: Admin/ContactMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,PhoneNumber,Company,Message")] ContactMessage contactMessage)
        {
            if (id != contactMessage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contactMessage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactMessageExists(contactMessage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)).WithSuccess("Message has been updated", null);
            }
            return View(contactMessage).WithError("Please fix all errors.", null);
        }

        // GET: Admin/ContactMessages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactMessage = await _context.ContactMessages.OrderBy(x => x.Id)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactMessage == null)
            {
                return NotFound();
            }

            return View(contactMessage);
        }

        // POST: Admin/ContactMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contactMessage = await _context.ContactMessages.FindAsync(id);
            _context.ContactMessages.Remove(contactMessage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)).WithSuccess("Message has been deleted.", null);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.ContactMessages
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.ContactMessages.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }




        private bool ContactMessageExists(int id)
        {
            return _context.ContactMessages.Any(e => e.Id == id);
        }
    }
}
