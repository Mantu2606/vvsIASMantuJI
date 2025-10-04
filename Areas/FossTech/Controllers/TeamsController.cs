using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using FossTech.Data;
using FossTech.Models;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class TeamsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/Teams
        public async Task<IActionResult> Index()
        {
            return View(await _context.Teams
                .OrderBy(x => x.SortOrder)
                .ToListAsync());
        }

        // GET: FossTech/Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: FossTech/Teams/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FossTech/Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile Image, Team team)
        {

            var imageName = "";
            if (Image != null)
            {
                imageName = Guid.NewGuid() + Path.GetExtension(Image.FileName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/team")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/team"));
                }

                //Get url To Save
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/team", imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }
            }


            team.Image = string.IsNullOrEmpty(imageName) ? null : $"/img/team/{imageName}";

            if (ModelState.IsValid)
            {
                _context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        // GET: FossTech/Teams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        // POST: FossTech/Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile Image, Team team)
        {
            if (id != team.Id)
            {
                return NotFound();
            }

            var thisMember = await _context.Teams.FirstOrDefaultAsync(i => i.Id == team.Id);
            if (thisMember == null)
            {
                return NotFound();
            }

            if (Image != null)
            {
                var mainImageName = Guid.NewGuid() + Path.GetExtension(Image.FileName);

                //Get url To Save
                var mainImageSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/team", mainImageName);

                await using (var stream = new FileStream(mainImageSavePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                thisMember.Image = $"/img/team/{mainImageName}";
            }
            if (ModelState.IsValid)
            {
                try
                {
                   
                    thisMember.Name = team.Name;
                    thisMember.Subject = team.Subject;
                    thisMember.SortOrder = team.SortOrder;
                    thisMember.Experience = team.Experience;
                    thisMember.Designation = team.Designation;
                    thisMember.Description = team.Description;
                    thisMember.SortOrder = team.SortOrder;
  


                    _context.Update(thisMember);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.Id))
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
            return View(team);
        }

        // GET: FossTech/Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: FossTech/Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.Teams
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.Teams.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
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

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
    }
}
