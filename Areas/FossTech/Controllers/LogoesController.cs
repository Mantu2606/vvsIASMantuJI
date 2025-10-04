using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class LogoesController : Controller
    {

        private readonly ApplicationDbContext _context;

        public LogoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Logoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Logos.ToListAsync());
        }

        // GET: Admin/Logoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logo = await _context.Logos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (logo == null)
            {
                return NotFound();
            }

            return View(logo);
        }

        // GET: Admin/Logoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Logoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile logo, IFormFile favicon, Logo post)
        {
            if (ModelState.IsValid)
            {
                if (logo != null)
                {
                    var HeaderImageName = Guid.NewGuid() + Path.GetExtension(logo.FileName);

                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos")))
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos"));
                    }

                    var HeaderSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos", HeaderImageName);

                    await using (var stream = new FileStream(HeaderSavePath, FileMode.Create))
                    {
                        logo.CopyTo(stream);
                    }

                    post.HeaderLogo = $"/img/Logos/{HeaderImageName}";
                }

                if (favicon != null)
                {
                    var FavImageName = Guid.NewGuid() + Path.GetExtension(favicon.FileName);
                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos")))
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos"));
                    }

                    var FaviconSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos", FavImageName);


                    await using (var stream = new FileStream(FaviconSavePath, FileMode.Create))
                    {
                        favicon.CopyTo(stream);
                    }

                    post.Favicon = $"/img/Logos/{FavImageName}";
                }

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Admin/Logoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logo = await _context.Logos.FindAsync(id);
            if (logo == null)
            {
                return NotFound();
            }
            return View(logo);
        }

        // POST: Admin/Logoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(268435456)]
        public async Task<IActionResult> Edit(int id, IFormFile logo, IFormFile favicon, Logo post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }
            var thisPost = await _context.Logos.FirstAsync(p => p.Id == post.Id);

            if (thisPost == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid) return View(post);

            if (logo != null)
            {
                //Set Key Name
                var imageName = Guid.NewGuid() + Path.GetExtension(logo.FileName);


                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos"));
                }

                //Get url To Save
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos", imageName);


                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    logo.CopyTo(stream);

                }

                thisPost.HeaderLogo = $"/img/Logos/{imageName}";


            }
            if (favicon != null)
            {
                var FavImageName = Guid.NewGuid() + Path.GetExtension(favicon.FileName);
                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos"));
                }

                var FaviconSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos", FavImageName);


                await using (var stream = new FileStream(FaviconSavePath, FileMode.Create))
                {
                    favicon.CopyTo(stream);
                }

                thisPost.Favicon = $"/img/Logos/{FavImageName}";
            }
            try
            {

                _context.Update(thisPost);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogoExists(post.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Logoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logo = await _context.Logos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (logo == null)
            {
                return NotFound();
            }

            return View(logo);
        }

        // POST: Admin/Logoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var logo = await _context.Logos.FindAsync(id);
            _context.Logos.Remove(logo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LogoExists(int id)
        {
            return _context.Logos.Any(e => e.Id == id);
        }
    }
}
