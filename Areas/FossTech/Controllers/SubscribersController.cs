using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models;
using Fingers10.ExcelExport.ActionResults;
using System.Collections.Generic;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SubscribersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubscribersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Subscribers
        public async Task<IActionResult> Index()
        {
            return View(await _context.NewsSubscribers.ToListAsync());
        }

        public async Task<IActionResult> ExporttoExcel()
        {
            var results = await _context.NewsSubscribers.ToListAsync();
            return new ExcelResult<NewsSubscriber>(results, "Demo Sheet Name", "Fingers10");

        }


        // GET: Admin/Subscribers/Details/5
        //        public async Task<IActionResult> Details(int? id)
        //        {
        //            if (id == null)
        //            {
        //                return NotFound();
        //            }
        //
        //            var subscriber = await _context.Subscribers
        //                .FirstOrDefaultAsync(m => m.Id == id);
        //            if (subscriber == null)
        //            {
        //                return NotFound();
        //            }
        //
        //            return View(subscriber);
        //        }

        // GET: Admin/Subscribers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Subscribers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Status,Created")] NewsSubscriber subscriber)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subscriber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subscriber);
        }

        // GET: Admin/Subscribers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriber = await _context.NewsSubscribers.FindAsync(id);
            if (subscriber == null)
            {
                return NotFound();
            }
            return View(subscriber);
        }

        // POST: Admin/Subscribers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Status,Created")] NewsSubscriber subscriber)
        {
            if (id != subscriber.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscriber);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriberExists(subscriber.Id))
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
            return View(subscriber);
        }

        // GET: Admin/Subscribers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriber = await _context.NewsSubscribers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subscriber == null)
            {
                return NotFound();
            }

            return View(subscriber);
        }

        // POST: Admin/Subscribers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subscriber = await _context.NewsSubscribers.FindAsync(id);
            _context.NewsSubscribers.Remove(subscriber);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.NewsSubscribers
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.NewsSubscribers.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool SubscriberExists(int id)
        {
            return _context.NewsSubscribers.Any(e => e.Id == id);
        }
    }
}
