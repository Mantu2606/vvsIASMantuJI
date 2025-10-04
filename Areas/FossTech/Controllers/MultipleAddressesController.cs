using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    public class MultipleAddressesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MultipleAddressesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/MultipleAddresses
        public async Task<IActionResult> Index()
        {
            return View(await _context.MultipleAddresses.OrderBy(x => x.SortOrder).ToListAsync());
        }

        // GET: FossTech/MultipleAddresses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var multipleAddress = await _context.MultipleAddresses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (multipleAddress == null)
            {
                return NotFound();
            }

            return View(multipleAddress);
        }

        // GET: FossTech/MultipleAddresses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FossTech/MultipleAddresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( MultipleAddress multipleAddress)
        {
            if (ModelState.IsValid)
            {
                _context.Add(multipleAddress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(multipleAddress);
        }

        // GET: FossTech/MultipleAddresses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var multipleAddress = await _context.MultipleAddresses.FindAsync(id);
            if (multipleAddress == null)
            {
                return NotFound();
            }
            return View(multipleAddress);
        }

        // POST: FossTech/MultipleAddresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MultipleAddress multipleAddress)
        {
            if (id != multipleAddress.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(multipleAddress);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MultipleAddressExists(multipleAddress.Id))
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
            return View(multipleAddress);
        }

        // GET: FossTech/MultipleAddresses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var multipleAddress = await _context.MultipleAddresses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (multipleAddress == null)
            {
                return NotFound();
            }

            return View(multipleAddress);
        }

        // POST: FossTech/MultipleAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var multipleAddress = await _context.MultipleAddresses.FindAsync(id);
            _context.MultipleAddresses.Remove(multipleAddress);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.MultipleAddresses
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.MultipleAddresses.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool MultipleAddressExists(int id)
        {
            return _context.MultipleAddresses.Any(e => e.Id == id);
        }
    }
}
