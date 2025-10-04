using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models.StudyMaterialModels;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using FossTech.Models;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles ="Admin,SuperAdmin")]
    public class StudyMaterialsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudyMaterialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var studyMaterials = await _context.StudyMaterials
                .Include(x => x.PDFs)
                .Include(sm => sm.Board)
                .Include(sm => sm.Standard)
                .Include(sm => sm.Subject)
                .Include(sm => sm.Chapter)
                .ToListAsync();

            return View(studyMaterials);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StudyMaterials == null)
            {
                return NotFound();
            }

            var studyMaterial = await _context.StudyMaterials
         .Include(sm => sm.Board)
         .Include(sm => sm.Standard)
         .Include(sm => sm.Subject)
         .Include(sm => sm.Chapter)
         .FirstOrDefaultAsync(sm => sm.Id == id);

            if (studyMaterial == null)
            {
                return NotFound();
            }

            return View(studyMaterial);
        }

        public IActionResult Create(int? boardId, int? standardId, int? subjectId)
        {
            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name", boardId);

            if (boardId.HasValue)
            {
                ViewData["StandardId"] = new SelectList(_context.Standards.Where(s => s.BoardId == boardId), "Id", "Name", standardId);
            }
            else
            {
                ViewData["StandardId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            if (standardId.HasValue)
            {
                ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(s => s.StandardId == standardId), "Id", "Name", subjectId);
            }
            else
            {
                ViewData["SubjectId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            if (subjectId.HasValue)
            {
                ViewData["ChapterId"] = new SelectList(_context.Chapters.Where(c => c.SubjectId == subjectId), "Id", "Name");
            }
            else
            {
                ViewData["ChapterId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<IFormFile> PDFs, IFormFile formImage, StudyMaterial studyMaterial)
        {
            if (ModelState.IsValid)
            {
                if (formImage == null)
                {
                    ModelState.AddModelError("Thumbnail Image", "Image is required.");
                    return View(studyMaterial);
                }
                var imageNamee = Guid.NewGuid() + Path.GetExtension(formImage.FileName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/studyMaterial")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/studyMaterial"));
                }

                //Get url To Save
                var savePathh = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/studyMaterial", imageNamee);

                await using (var stream = new FileStream(savePathh, FileMode.Create))
                {
                    formImage.CopyTo(stream);
                }

                studyMaterial.ThumbnailImage = $"/img/studyMaterial/{imageNamee}";

                studyMaterial.CreatedAt = DateTime.Now;
                studyMaterial.LastModified = DateTime.Now;
                _context.Add(studyMaterial);
                await _context.SaveChangesAsync();


                if (PDFs != null)
                {
                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf/studyMaterial")))
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                            "wwwroot/pdf/studyMaterial"));
                    }

                    foreach (IFormFile item in PDFs)
                    {
                        //Set Key Name
                        var imageName = Guid.NewGuid() + Path.GetExtension(item.FileName);

                        //Get url To Save
                        var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf/studyMaterial", imageName);

                        await using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await item.CopyToAsync(stream);
                        }

                        var pdf = new StudyMaterialPDF()
                        {
                            StudyMaterialId = studyMaterial.Id,
                            PDF = $"/pdf/studyMaterial/{imageName}"
                        };

                        _context.Add(pdf);
                    }

                    await _context.SaveChangesAsync();
                }



                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", studyMaterial.ProductId);
            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name", studyMaterial.BoardId);
            ViewData["StandardId"] = new SelectList(_context.Standards.Where(s => s.BoardId == studyMaterial.BoardId), "Id", "Name", studyMaterial.StandardId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(s => s.StandardId == studyMaterial.StandardId), "Id", "Name", studyMaterial.SubjectId);
            ViewData["ChapterId"] = new SelectList(_context.Chapters.Where(c => c.SubjectId == studyMaterial.SubjectId), "Id", "Name", studyMaterial.ChapterId);
            return View(studyMaterial);
        }

        // GET: FossTech/StudyMaterials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StudyMaterials == null)
            {
                return NotFound();
            }

            var studyMaterial = await _context.StudyMaterials.Include(x => x.PDFs).FirstOrDefaultAsync(x => x.Id == id);
            if (studyMaterial == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", studyMaterial.ProductId);
            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name", studyMaterial.BoardId);
            ViewData["StandardId"] = new SelectList(_context.Standards.Where(s => s.BoardId == studyMaterial.BoardId), "Id", "Name", studyMaterial.StandardId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(s => s.StandardId == studyMaterial.StandardId), "Id", "Name", studyMaterial.SubjectId);
            ViewData["ChapterId"] = new SelectList(_context.Chapters.Where(c => c.SubjectId == studyMaterial.SubjectId), "Id", "Name", studyMaterial.ChapterId);
            return View(studyMaterial);
        }

        // POST: FossTech/StudyMaterials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, List<IFormFile> PDFs, IFormFile image, StudyMaterial studyMaterial)
        {
            if (id != studyMaterial.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                var thisMember = await _context.StudyMaterials.FirstOrDefaultAsync(i => i.Id == studyMaterial.Id);
                if (thisMember==null)
                {
                    return NotFound();
                }

                if (image != null)
                {
                    //Set Key Name
                    var imageNamee = Guid.NewGuid() + Path.GetExtension(image.FileName);

                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/studyMaterial")))
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/studyMaterial"));
                    }

                    //Get url To Save
                    var savePathh = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/studyMaterial", imageNamee);

                    await using (var stream = new FileStream(savePathh, FileMode.Create))
                    {
                        image.CopyTo(stream);
                    }

                    thisMember.ThumbnailImage = $"/img/studyMaterial/{imageNamee}";
                }
                try
                {
                    if (PDFs != null)
                    {
                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf/studyMaterial")))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                                "wwwroot/pdf/studyMaterial"));
                        }

                        foreach (IFormFile item in PDFs)
                        {
                            //Set Key Name
                            var imageName = Guid.NewGuid() + Path.GetExtension(item.FileName);

                            //Get url To Save
                            var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf/studyMaterial", imageName);

                            await using (var stream = new FileStream(savePath, FileMode.Create))
                            {
                                await item.CopyToAsync(stream);
                            }

                            var pdf = new StudyMaterialPDF()
                            {
                                StudyMaterialId = studyMaterial.Id,
                                PDF = $"/pdf/studyMaterial/{imageName}"
                            };

                            _context.Add(pdf);
                        }

                        await _context.SaveChangesAsync();
                    }
                    thisMember.Name = studyMaterial.Name;
                    thisMember.SortOrder = studyMaterial.SortOrder;
                    thisMember.ProductId = studyMaterial.ProductId;
                    thisMember.BoardId = studyMaterial.BoardId;
                    thisMember.StandardId = studyMaterial.StandardId;
                    thisMember.SubjectId = studyMaterial.SubjectId;
                    thisMember.ChapterId = studyMaterial.ChapterId;
                    thisMember.IsPaid = studyMaterial.IsPaid;
                    thisMember.BasePrice = studyMaterial.BasePrice;
                    thisMember.DiscountAmount = studyMaterial.DiscountAmount;
                    _context.Update(thisMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudyMaterialExists(studyMaterial.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", studyMaterial.ProductId);
            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name", studyMaterial.BoardId);
            ViewData["StandardId"] = new SelectList(_context.Standards.Where(s => s.BoardId == studyMaterial.BoardId), "Id", "Name", studyMaterial.StandardId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(s => s.StandardId == studyMaterial.StandardId), "Id", "Name", studyMaterial.SubjectId);
            ViewData["ChapterId"] = new SelectList(_context.Chapters.Where(c => c.SubjectId == studyMaterial.SubjectId), "Id", "Name", studyMaterial.ChapterId);
            return View(studyMaterial);
        }

        // GET: FossTech/StudyMaterials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StudyMaterials == null)
            {
                return NotFound();
            }

            var studyMaterial = await _context.StudyMaterials
                //.Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studyMaterial == null)
            {
                return NotFound();
            }

            return View(studyMaterial);
        }

        // POST: FossTech/StudyMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StudyMaterials == null)
            {
                return Problem("Entity set 'ApplicationDbContext.StudyMaterials'  is null.");
            }
            var studyMaterial = await _context.StudyMaterials.FindAsync(id);
            if (studyMaterial != null)
            {
                _context.StudyMaterials.Remove(studyMaterial);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudyMaterialExists(int id)
        {
          return _context.StudyMaterials.Any(e => e.Id == id);
        }

        public async Task<IActionResult> DeleteStudyMaterialPdf(int pdfId, int StudyMaterialId)
        {
            var studyMaterialPdf = await _context.StudyMaterialPDFs.FindAsync(pdfId);
            _context.StudyMaterialPDFs.Remove(studyMaterialPdf);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = StudyMaterialId });
        }

        public async Task<IActionResult> GetStandardsByBoardId(int boardId)
        {
            var standards = await _context.Standards.Where(s => s.BoardId == boardId)
                                              .Select(s => new { s.Id, s.Name })
                                              .ToListAsync();
            return Json(standards);
        }

        public async Task<IActionResult> GetSubjectsByStandardId(int standardId)
        {
            var subjects =await _context.Subjects.Where(s => s.StandardId == standardId)
                                            .Select(s => new { s.Id, s.Name })
                                            .ToListAsync();
            return Json(subjects);
        }

        public async Task<IActionResult> GetChaptersBySubjectId(int subjectId)
        {
            var chapters =await _context.Chapters.Where(c => c.SubjectId == subjectId)
                                            .Select(c => new { c.Id, c.Name })
                                            .ToListAsync();
            return Json(chapters);
        }

    }
}
