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
    public class DemoLecturesEnquiriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DemoLecturesEnquiriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/FreeCourses
        public async Task<IActionResult> Index()
        {
            var demolec = await _context.DemoLecturesEnquiries
                                        .Include(x => x.Branch)
                                        .Include(x => x.Courses)
                                            .ThenInclude(x => x.Product)
                                        .OrderByDescending(x => x.CreatedAt)
                                        .ToListAsync();

            return View(demolec);
        }



        // GET: FossTech/FreeCourses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DemoLecturesEnquiries == null)
            {
                return NotFound();
            }

            var freeCourse = await _context.DemoLecturesEnquiries
                .Include(x => x.Branch).Include(x => x.Courses).ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (freeCourse == null)
            {
                return NotFound();
            }

            return View(freeCourse);
        }


        // GET: FossTech/FreeCourses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FossTech/FreeCourses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DemoLecturesEnquiry demoLectures)
        {
            if (ModelState.IsValid)
            {
                _context.Add(demoLectures);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(demoLectures);
        }


        // GET: FossTech/FreeCourses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DemoLecturesEnquiries == null)
            {
                return NotFound();
            }

            var freeCourse = await _context.DemoLecturesEnquiries.FindAsync(id);
            if (freeCourse == null)
            {
                return NotFound();
            }
            return View(freeCourse);
        }

        // POST: FossTech/FreeCourses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DemoLecturesEnquiry demoLectures)
        {
            if (id != demoLectures.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(demoLectures);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FreeCourseExists(demoLectures.Id))
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
            return View(demoLectures);
        }

        // GET: FossTech/FreeCourses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DemoLecturesEnquiries == null)
            {
                return NotFound();
            }

            var freeCourse = await _context.DemoLecturesEnquiries.Include(x => x.Branch).Include(x => x.Courses).ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (freeCourse == null)
            {
                return NotFound();
            }

            return View(freeCourse);
        }

        // POST: FossTech/FreeCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DemoLecturesEnquiries == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FreeCourses'  is null.");
            }
            var freeCourse = await _context.DemoLecturesEnquiries.FindAsync(id);
            if (freeCourse != null)
            {
                _context.DemoLecturesEnquiries.Remove(freeCourse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.DemoLecturesEnquiries
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.DemoLecturesEnquiries.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool FreeCourseExists(int id)
        {
            return (_context.DemoLecturesEnquiries?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
