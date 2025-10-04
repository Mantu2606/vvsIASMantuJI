using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class FAQsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FAQsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/FAQs
        public async Task<IActionResult> Index()
        {
            return View(await _context.FAQs.ToListAsync());
        }

        // GET: Admin/FAQs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.FAQs.OrderBy(x=>x.Id)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // GET: Admin/FAQs/Create
        public IActionResult Create()
        {
            ViewData["Sections"] = new MultiSelectList(_context.Sections, "Id", "Name");
            return View();
        }

        // POST: Admin/FAQs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FAQ fAQ)
        {
            ViewData["Sections"] = new MultiSelectList(_context.Sections, "Id", "Name");
            if (ModelState.IsValid)
            {
                _context.Add(fAQ);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fAQ);
        }

        // GET: Admin/FAQs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.FAQs.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }
            ViewData["Sections"] = new MultiSelectList(_context.Sections, "Id", "Name");
            return View(page);
        }

        // POST: Admin/FAQs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FAQ fAQ)
        {
            ViewData["Sections"] = new MultiSelectList(_context.Sections, "Id", "Name");
            if (id != fAQ.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fAQ);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PageExists(fAQ.Id))
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
            return View(fAQ);
        }

        // GET: Admin/FAQs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.FAQs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // POST: Admin/FAQs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var page = await _context.FAQs.FindAsync(id);
            _context.FAQs.Remove(page);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ImageUpload(IFormFile file)
        {
            if (file != null)
            {
                //Set Key Name
                var imageName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/uploads")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/uploads"));
                }

                //Get url To Save
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/uploads", imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return Content($"/img/uploads/{imageName}");
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.FAQs
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.FAQs.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        public async Task<IActionResult> ResetMenu()
        {
            var faqs = await _context.FAQs.ToListAsync();

            _context.RemoveRange(faqs);

            await _context.SaveChangesAsync();

            var bussinessProfile = await _context.BusinessProfile.FirstOrDefaultAsync();
            var branches = await _context.Branches.FirstOrDefaultAsync();

            await _context.AddRangeAsync(new List<FAQ>
            {
                new FAQ
                {
                    Question = "Where it is located?",
                    Slug = "where-it-is-located",
                    Answer = $"It is located in {branches.Address}",
                    Order = 1
                },
                new FAQ
                {
                    Question = "What are your working hours?",
                    Slug = "what-are-your-working-hours",
                    Answer = $"Our working hours are as follows: {bussinessProfile.WorkingHours}",
                    Order = 2
                },
                new FAQ
                {
                    Question = "What is your WhatsApp No/ Mobile No/ Contact No?",
                    Slug = "what-is-your-whatsapp-no-mobile-no-contact-no",
                    Answer = $"You can contact us through the following numbers: Mobile Number: {bussinessProfile.RegisteredContactNumber}, WhatsApp Number: {bussinessProfile.WhatsAppNumber} If you have any further questions or concerns, please do not hesitate to ask. We're always here to help.",
                    Order = 3
                },
                new FAQ
                {
                    Question = "What is your Email Id?",
                    Slug = "what-is-your-email-id",
                    Answer = $"You can contact us through the following Emails: {bussinessProfile.EmailAddress} - {bussinessProfile.AlternateEmailAddress} If you have any further questions or concerns, please do not hesitate to ask. We're always here to help.",
                    Order = 4
                },
                new FAQ
                {
                    Question = "Are you on social media?",
                    Slug = "are-you-on-social-media",
                    Answer = $"You can follow us on Facebook: {bussinessProfile.FacebookPageLink} , Twitter: {bussinessProfile.TwitterLink} , Instagram: {bussinessProfile.InstagramPageLink} If you have any further questions or concerns, please do not hesitate to ask. We're always here to help.",
                    Order = 5
                },

            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool PageExists(int id)
        {
            return _context.FAQs.Any(e => e.Id == id);
        }
    }
}
