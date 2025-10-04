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
    public class BranchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BranchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/Branches
        public async Task<IActionResult> Index()
        {
              return View(await _context.Branches
                  .Include(b => b.Section)
                  .OrderBy(x => x.SortOrder).ToListAsync());
        }

        // GET: FossTech/Branches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Branches == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches
                .FirstOrDefaultAsync(m => m.Id == id);
            if (branch == null)
            {
                return NotFound();
            }

            return View(branch);
        }

        // GET: FossTech/Branches/Create
        public IActionResult Create()
        {
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");

            return View();
        }

        // POST: FossTech/Branches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Branch branch)
        {
            if (ModelState.IsValid)
            {
                _context.Add(branch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");

            return View(branch);
        }

        // GET: FossTech/Branches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Branches == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");

            return View(branch);
        }

        // POST: FossTech/Branches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Branch branch)
        {
            if (id != branch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var thisBranch = await _context.Branches.FirstOrDefaultAsync(i => i.Id == branch.Id);

                    if (thisBranch == null)
                    {
                        return NotFound();
                    }

                    thisBranch.BranchName = branch.BranchName;
                    thisBranch.SortOrder = branch.SortOrder;
                    thisBranch.SectionId = branch.SectionId;
                    thisBranch.Address = branch.Address;
                    thisBranch.Address  = branch.Address;
                    thisBranch.GoogleMapLink = branch.GoogleMapLink; 
                    thisBranch.GoogleIframe = branch.GoogleIframe;
                    thisBranch.Mobile = branch.Mobile;
                    thisBranch.Email = branch.Email;


                    _context.Update(thisBranch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BranchExists(branch.Id))
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
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");

            return View(branch);
        }

        // GET: FossTech/Branches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Branches == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches
                .FirstOrDefaultAsync(m => m.Id == id);
            if (branch == null)
            {
                return NotFound();
            }

            return View(branch);
        }

        // POST: FossTech/Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Branches == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Branches'  is null.");
            }
            var branch = await _context.Branches.FindAsync(id);
            if (branch != null)
            {
                _context.Branches.Remove(branch);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.Branches
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.Branches.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool BranchExists(int id)
        {
          return _context.Branches.Any(e => e.Id == id);
        }
    }
}
