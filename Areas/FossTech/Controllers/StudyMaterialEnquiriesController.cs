using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models;
using Microsoft.AspNetCore.Authorization;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles= "Admin,SuperAdmin")]
    public class StudyMaterialEnquiriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudyMaterialEnquiriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var studyMaterialEnquiries = await _context.studyMaterialEnquiries.OrderByDescending(x=>x.CreatedAt).ToListAsync();

            return View(studyMaterialEnquiries);
        }

        // GET: FossTech/StudyMaterialEnquiries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.studyMaterialEnquiries == null)
            {
                return NotFound();
            }

            var studyMaterialEnquiry = await _context.studyMaterialEnquiries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studyMaterialEnquiry == null)
            {
                return NotFound();
            }

            return View(studyMaterialEnquiry);
        }

        // GET: FossTech/StudyMaterialEnquiries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FossTech/StudyMaterialEnquiries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,MobileNumber,Name,MaterialName,Message,CreatedAt")] StudyMaterialEnquiry studyMaterialEnquiry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studyMaterialEnquiry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(studyMaterialEnquiry);
        }

        // GET: FossTech/StudyMaterialEnquiries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.studyMaterialEnquiries == null)
            {
                return NotFound();
            }

            var studyMaterialEnquiry = await _context.studyMaterialEnquiries.FindAsync(id);
            if (studyMaterialEnquiry == null)
            {
                return NotFound();
            }
            return View(studyMaterialEnquiry);
        }

        // POST: FossTech/StudyMaterialEnquiries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,MobileNumber,Name,MaterialName,Message,CreatedAt")] StudyMaterialEnquiry studyMaterialEnquiry)
        {
            if (id != studyMaterialEnquiry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studyMaterialEnquiry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudyMaterialEnquiryExists(studyMaterialEnquiry.Id))
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
            return View(studyMaterialEnquiry);
        }

        // GET: FossTech/StudyMaterialEnquiries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.studyMaterialEnquiries == null)
            {
                return NotFound();
            }

            var studyMaterialEnquiry = await _context.studyMaterialEnquiries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studyMaterialEnquiry == null)
            {
                return NotFound();
            }

            return View(studyMaterialEnquiry);
        }

        // POST: FossTech/StudyMaterialEnquiries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.studyMaterialEnquiries == null)
            {
                return Problem("Entity set 'ApplicationDbContext.studyMaterialEnquiries'  is null.");
            }
            var studyMaterialEnquiry = await _context.studyMaterialEnquiries.FindAsync(id);
            if (studyMaterialEnquiry != null)
            {
                _context.studyMaterialEnquiries.Remove(studyMaterialEnquiry);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest("No records selected for deletion.");
            }
            foreach (var id in ids)
            {
                var studyMaterialEnquiry = await _context.studyMaterialEnquiries.FindAsync(id);
                if (studyMaterialEnquiry != null)
                {
                    _context.studyMaterialEnquiries.Remove(studyMaterialEnquiry);
                }
            }
            _context.SaveChanges();
            return Ok(); 
        }
        private bool StudyMaterialEnquiryExists(int id)
        {
          return _context.studyMaterialEnquiries.Any(e => e.Id == id);
        }
    }
}
