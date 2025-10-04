using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models;
using Fingers10.ExcelExport.ActionResults;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SellerApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SellerApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/SellerApplications
        public async Task<IActionResult> Index()
        {
            return View(await _context.SellerApplications.ToListAsync());
        }


        public async Task<IActionResult> ExporttoExcel()
        {
            var results = await _context.SellerApplications.ToListAsync();
            return new ExcelResult<SellerApplication>(results, "Demo Sheet Name", "Fingers10");
            
        }




        // GET: FossTech/SellerApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sellerApplication = await _context.SellerApplications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sellerApplication == null)
            {
                return NotFound();
            }

            return View(sellerApplication);
        }

        // GET: FossTech/SellerApplications/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FossTech/SellerApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile DocumentImg, SellerApplication sellerApplication)
        {
            if (ModelState.IsValid)
            {
                if (DocumentImg == null)
                {
                    ModelState.AddModelError("DocumentImage", "Document Image is required.");
                    return View(sellerApplication);
                }

                var anyApp = await _context.SellerApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Email == sellerApplication.Email);

                if (anyApp != null)
                {
                    ModelState.AddModelError("Email", "Application with this email already exist.");
                    return View(sellerApplication);
                }

                var imageName = Guid.NewGuid() + Path.GetExtension(DocumentImg.FileName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/SellerApplication")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/SellerApplication"));
                }

                //Get url To Save
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/SellerApplication", imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await DocumentImg.CopyToAsync(stream);
                }
                sellerApplication.DocumentImage = $"/img/SellerApplication/{imageName}";

                sellerApplication.CreatedAt = DateTime.Now;

                _context.Add(sellerApplication);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sellerApplication);
        }

        // GET: FossTech/SellerApplications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sellerApplication = await _context.SellerApplications.FindAsync(id);
            if (sellerApplication == null)
            {
                return NotFound();
            }
            return View(sellerApplication);
        }

        // POST: FossTech/SellerApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile DocumentImg, SellerApplication sellerApplication)
        {
            if (id != sellerApplication.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var mySellerApplication = await _context.SellerApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);


                    if (DocumentImg != null)
                    {                                               

                        var imageName = Guid.NewGuid() + Path.GetExtension(DocumentImg.FileName);

                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/SellerApplication")))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/SellerApplication"));
                        }

                        //Get url To Save
                        var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/SellerApplication", imageName);

                        await using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await DocumentImg.CopyToAsync(stream);
                        }
                        sellerApplication.DocumentImage = $"/img/SellerApplication/{imageName}";

                    } else
                    {
                        sellerApplication.DocumentImage = mySellerApplication.DocumentImage;
                    }
                    sellerApplication.CreatedAt = mySellerApplication.CreatedAt;

                    _context.Update(sellerApplication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SellerApplicationExists(sellerApplication.Id))
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
            return View(sellerApplication);
        }

        // GET: FossTech/SellerApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sellerApplication = await _context.SellerApplications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sellerApplication == null)
            {
                return NotFound();
            }

            return View(sellerApplication);
        }

        // POST: FossTech/SellerApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sellerApplication = await _context.SellerApplications.FindAsync(id);
            _context.SellerApplications.Remove(sellerApplication);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SellerApplicationExists(int id)
        {
            return _context.SellerApplications.Any(e => e.Id == id);
        }
    }
}
