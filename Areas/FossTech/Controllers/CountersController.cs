using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    public class CountersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CountersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/Counters
        public async Task<IActionResult> Index()
        {
              return View(await _context.Counters.ToListAsync());
        }

        // GET: FossTech/Counters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Counters == null)
            {
                return NotFound();
            }

            var counter = await _context.Counters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (counter == null)
            {
                return NotFound();
            }

            return View(counter);
        }

        // GET: FossTech/Counters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FossTech/Counters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Counter counter , IFormFile icon)
        {
            if (ModelState.IsValid)
            {
                if (icon == null)
                {
                    ModelState.AddModelError("Image", "Image is required.");
                    return View(counter);
                }
                var imagename = Guid.NewGuid() + Path.GetExtension(icon.FileName);
                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Icon")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Icon"));
                }
                var savepath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/img/Icon",imagename);
                await using (var stream = new FileStream(savepath, FileMode.Create))
                {
                    icon.CopyTo(stream);
                }
                counter.Icon = $"/img/Icon/{imagename}";
                _context.Add(counter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(counter);
        }

        // GET: FossTech/Counters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Counters == null)
            {
                return NotFound();
            }

            var counter = await _context.Counters.FindAsync(id);
            if (counter == null)
            {
                return NotFound();
            }
            return View(counter);
        }

        // POST: FossTech/Counters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Counter counter,IFormFile icon)
        {
            if (id != counter.Id)
            {
                return NotFound();
            }
            var thispost= await _context.Counters.FirstAsync(x =>x.Id == counter.Id);
            if (thispost == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (icon != null)
                {
                    var imagename = Guid.NewGuid() + Path.GetExtension(icon.FileName);
                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Icon")))
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Icon"));
                    }
                    var savepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Icon", imagename);
                    await using (var stream = new FileStream(savepath, FileMode.Create))
                    {
                        icon.CopyTo(stream);
                    }
                    thispost.Icon = $"/img/Icon/{imagename}";
                }
                
                try
                {
                    thispost.Title= counter.Title;
                    thispost.Number = counter.Number;   
                    thispost.SortOreder= counter.SortOreder;
                    _context.Update(thispost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CounterExists(counter.Id))
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
            return View(counter);
        }

        // GET: FossTech/Counters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Counters == null)
            {
                return NotFound();
            }

            var counter = await _context.Counters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (counter == null)
            {
                return NotFound();
            }

            return View(counter);
        }

        // POST: FossTech/Counters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Counters == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Counters'  is null.");
            }
            var counter = await _context.Counters.FindAsync(id);
            if (counter != null)
            {
                _context.Counters.Remove(counter);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.Counters
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.Counters.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool CounterExists(int id)
        {
          return _context.Counters.Any(e => e.Id == id);
        }
    }
}
