using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Extensions;
using FossTech.Models;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class LoginPageImagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginPageImagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/LoginPageImages
        public async Task<IActionResult> Index()
        {
            return View(await _context.LoginPageImages
                .OrderBy(x => x.Order)
                .ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(268435456)]
        public async Task<IActionResult> Create(List<IFormFile> images)
        {
            if (images == null || images.Count <= 0) return RedirectToAction(nameof(Index));

            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/LoginPageImages")))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot/img/LoginPageImages"));
            }

            foreach (IFormFile item in images)
            {
                //Set Key Name
                var imageName = Guid.NewGuid() + Path.GetExtension(item.FileName);

                //Get url To Save
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/LoginPageImages", imageName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    item.CopyTo(stream);
                }

                var image = new LoginPageImage { Image = $"/img/LoginPageImages/{imageName}" };

                _context.Add(image);
                await _context.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Index)).WithSuccess("Slider image has been added.", null);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loginPageImage = await _context.LoginPageImages.FindAsync(id);
            if (loginPageImage == null)
            {
                return NotFound();
            }
            return View(loginPageImage);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile img, LoginPageImage loginPageImage)
        {
            var oldLoginPageImage = await _context.LoginPageImages.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (img != null)
            {
                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/LoginPageImages")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot/img/LoginPageImages"));
                }

                var imageName = Guid.NewGuid() + Path.GetExtension(img.FileName);

                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/LoginPageImages", imageName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    img.CopyTo(stream);
                }

                loginPageImage.Image = $"/img/LoginPageImages/{imageName}";
            }           
            else
            {
                loginPageImage.Image = oldLoginPageImage.Image;
            }

            _context.Update(loginPageImage);

            await _context.SaveChangesAsync();

            
            return RedirectToAction(nameof(Index)).WithSuccess("Slider image has been updated.", null);
        }

        // GET: Admin/LoginPageImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loginPageImage = await _context.LoginPageImages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loginPageImage == null)
            {
                return NotFound();
            }

            return View(loginPageImage);
        }

        // POST: Admin/LoginPageImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loginPageImage = await _context.LoginPageImages.FindAsync(id);
            _context.LoginPageImages.Remove(loginPageImage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)).WithSuccess("Login page image has been deleted.", null);
        }

        private bool LoginPageImageExists(int id)
        {
            return _context.LoginPageImages.Any(e => e.Id == id);
        }
    }
}
