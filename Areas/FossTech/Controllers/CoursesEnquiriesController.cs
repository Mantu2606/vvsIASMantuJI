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
    public class CoursesEnquiriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesEnquiriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/DepositAndLoanEnquiries
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CoursesEnquiries.Include(d => d.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FossTech/DepositAndLoanEnquiries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CoursesEnquiries == null)
            {
                return NotFound();
            }

            var depositAndLoanEnquiry = await _context.CoursesEnquiries
                .Include(d => d.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (depositAndLoanEnquiry == null)
            {
                return NotFound();
            }

            return View(depositAndLoanEnquiry);
        }

        // GET: FossTech/DepositAndLoanEnquiries/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: FossTech/DepositAndLoanEnquiries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,Name,Phone,Email")] CoursesEnquiry coursesEnquiry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coursesEnquiry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", coursesEnquiry.ProductId);
            return View(coursesEnquiry);
        }

        // GET: FossTech/DepositAndLoanEnquiries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CoursesEnquiries == null)
            {
                return NotFound();
            }

            var depositAndLoanEnquiry = await _context.CoursesEnquiries.FindAsync(id);
            if (depositAndLoanEnquiry == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", depositAndLoanEnquiry.ProductId);
            return View(depositAndLoanEnquiry);
        }

        // POST: FossTech/DepositAndLoanEnquiries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Name,Phone,Email")] CoursesEnquiry coursesEnquiry)
        {
            if (id != coursesEnquiry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coursesEnquiry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepositAndLoanEnquiryExists(coursesEnquiry.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", coursesEnquiry.ProductId);
            return View(coursesEnquiry);
        }

        // GET: FossTech/DepositAndLoanEnquiries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CoursesEnquiries == null)
            {
                return NotFound();
            }

            var depositAndLoanEnquiry = await _context.CoursesEnquiries
                .Include(d => d.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (depositAndLoanEnquiry == null)
            {
                return NotFound();
            }

            return View(depositAndLoanEnquiry);
        }

        // POST: FossTech/DepositAndLoanEnquiries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CoursesEnquiries == null)
            {
                return Problem("Entity set 'ApplicationDbContext.DepositAndLoanEnquiries'  is null.");
            }
            var depositAndLoanEnquiry = await _context.CoursesEnquiries.FindAsync(id);
            if (depositAndLoanEnquiry != null)
            {
                _context.CoursesEnquiries.Remove(depositAndLoanEnquiry);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.CoursesEnquiries
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.CoursesEnquiries.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool DepositAndLoanEnquiryExists(int id)
        {
          return _context.CoursesEnquiries.Any(e => e.Id == id);
        }
    }
}
