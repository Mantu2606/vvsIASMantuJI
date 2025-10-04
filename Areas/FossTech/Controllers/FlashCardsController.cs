using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models.StudyMaterialModels;
using Microsoft.AspNetCore.Authorization;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class FlashCardsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlashCardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/FlashCards
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = await _context.FlashCards
                                               .Include(f => f.Board)
                                               .Include(f => f.Chapter)
                                               .Include(f => f.Standard)
                                               .Include(f => f.Subject)
                                               .OrderBy(x=>x.CreatedAt)
                                               .ToListAsync();
            return View(applicationDbContext);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FlashCards == null)
            {
                return NotFound();
            }

            var flashCard = await _context.FlashCards
                .Include(f => f.Board)
                .Include(f => f.Chapter)
                .Include(f => f.Standard)
                .Include(f => f.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flashCard == null)
            {
                return NotFound();
            }

            return View(flashCard);
        }

        // GET: FossTech/FlashCards/Create
        public IActionResult Create(int? boardId, int? standardId, int? subjectId)
        {
            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name", boardId);
            if (boardId.HasValue)
            {
                ViewData["StandardId"] = new SelectList(_context.Standards.Where(x => x.BoardId == boardId), "Id", "Name", standardId);
            }
            else
            {
                ViewData["StandardId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }
            if (standardId.HasValue)
            {
                ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(x => x.StandardId == standardId), "Id", "Name", subjectId);
            }
            else
            {
                ViewData["SubjectId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }
            if (subjectId.HasValue)
            {
                ViewData["ChapterId"] = new SelectList(_context.Chapters.Where(s => s.SubjectId == standardId), "Id", "Name");
            }
            else
            {
                ViewData["ChapterId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FlashCard flashCard, IFormFile QuestionImage, IFormFile AnswerImage)
        {
            if (ModelState.IsValid)
            {
                var flashCardImagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "flashcard");

                if (!Directory.Exists(flashCardImagesDirectory))
                {
                    Directory.CreateDirectory(flashCardImagesDirectory);
                }

                if (QuestionImage != null)
                {
                    var questionImageName = Guid.NewGuid() + Path.GetExtension(QuestionImage.FileName);

                    var questionImagePath = Path.Combine(flashCardImagesDirectory, questionImageName);

                    await using (var stream = new FileStream(questionImagePath, FileMode.Create))
                    {
                        await QuestionImage.CopyToAsync(stream);
                    }

                    flashCard.QuestionImagePath = $"/img/flashcard/{questionImageName}";
                }
                if (AnswerImage != null)
                {
                    var answerImageName = Guid.NewGuid() + Path.GetExtension(AnswerImage.FileName);

                    var answerImagePath = Path.Combine(flashCardImagesDirectory, answerImageName);

                    await using (var stream = new FileStream(answerImagePath, FileMode.Create))
                    {
                        await AnswerImage.CopyToAsync(stream);
                    }

                    flashCard.AnswerImagePath = $"/img/flashcard/{answerImageName}";
                }

                _context.Add(flashCard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name", flashCard.BoardId);
            ViewData["StandardId"] = new SelectList(_context.Standards.Where(b => b.BoardId == flashCard.BoardId), "Id", "Name", flashCard.StandardId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(x => x.StandardId == flashCard.StandardId), "Id", "Name", flashCard.SubjectId);
            ViewData["ChapterId"] = new SelectList(_context.Chapters.Where(x => x.SubjectId == flashCard.SubjectId), "Id", "Name", flashCard.ChapterId);
            return View(flashCard);
        }

        // GET: FossTech/FlashCards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FlashCards == null)
            {
                return NotFound();
            }

            var flashCard = await _context.FlashCards.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id);
            if (flashCard == null)
            {
                return NotFound();
            }
            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name", flashCard.BoardId);
            ViewData["StandardId"] = new SelectList(_context.Standards.Where(b=>b.BoardId==flashCard.BoardId), "Id", "Name", flashCard.StandardId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(s=>s.StandardId==flashCard.StandardId), "Id", "Name", flashCard.SubjectId);
            ViewData["ChapterId"] = new SelectList(_context.Chapters.Where(s=>s.SubjectId==flashCard.SubjectId), "Id", "Name", flashCard.ChapterId);         
            return View(flashCard);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FlashCard flashCard, IFormFile QuestionImage, IFormFile AnswerImage)
        {
            if (id != flashCard.Id)
            {
                return NotFound();
            }
            var oldPost = await _context.FlashCards
                .Include(f => f.Board)
                .Include(f => f.Chapter)
                .Include(f => f.Standard)
                .Include(f => f.Subject)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ModelState.IsValid)
            {
                try
                {
                    if (QuestionImage != null && QuestionImage.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(flashCard.QuestionImagePath))
                        {
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", flashCard.QuestionImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        var flashCardImagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "flashcard");
                        if (!Directory.Exists(flashCardImagesDirectory))
                        {
                            Directory.CreateDirectory(flashCardImagesDirectory);
                        }

                        var questionImageName = Guid.NewGuid() + Path.GetExtension(QuestionImage.FileName);
                        var questionImagePath = Path.Combine(flashCardImagesDirectory, questionImageName);

                        await using (var stream = new FileStream(questionImagePath, FileMode.Create))
                        {
                            await QuestionImage.CopyToAsync(stream);
                        }

                        flashCard.QuestionImagePath = $"/img/flashcard/{questionImageName}";
                    }
                    else
                    {
                        // Preserve old image path if no new image uploaded
                        flashCard.QuestionImagePath = oldPost.QuestionImagePath;
                    }

                    // Answer Image Handling
                    if (AnswerImage != null && AnswerImage.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(flashCard.AnswerImagePath))
                        {
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", flashCard.AnswerImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        var flashCardImagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "flashcard");
                        if (!Directory.Exists(flashCardImagesDirectory))
                        {
                            Directory.CreateDirectory(flashCardImagesDirectory);
                        }

                        var answerImageName = Guid.NewGuid() + Path.GetExtension(AnswerImage.FileName);
                        var answerImagePath = Path.Combine(flashCardImagesDirectory, answerImageName);

                        await using (var stream = new FileStream(answerImagePath, FileMode.Create))
                        {
                            await AnswerImage.CopyToAsync(stream);
                        }

                        flashCard.AnswerImagePath = $"/img/flashcard/{answerImageName}";
                    }
                    else
                    {
                        // Preserve old image path if no new image uploaded
                        flashCard.AnswerImagePath = oldPost.AnswerImagePath;
                    }
                    oldPost.BoardId = flashCard.BoardId;
                    oldPost.StandardId = flashCard.StandardId;
                    oldPost.SubjectId = flashCard.SubjectId;
                    oldPost.ChapterId = flashCard.ChapterId;
                    oldPost.Name = flashCard.Name;
                    oldPost.Question = flashCard.Question;
                    oldPost.Answer = flashCard.Answer;
                    oldPost.SortOrder = flashCard.SortOrder;
                    oldPost.IsPaid = flashCard.IsPaid;
                    oldPost.BasePrice = flashCard.BasePrice;
                    oldPost.DiscountAmount = flashCard.DiscountAmount;

                    _context.Update(flashCard);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlashCardExists(flashCard.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while processing the image uploads: " + ex.Message);
                    return View(flashCard);
                }
            }

            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name", flashCard.BoardId);
            ViewData["StandardId"] = new SelectList(_context.Standards.Where(b => b.BoardId == flashCard.BoardId), "Id", "Name", flashCard.StandardId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(s=>s.StandardId==flashCard.StandardId), "Id", "Name", flashCard.SubjectId);
            ViewData["ChapterId"] = new SelectList(_context.Chapters.Where(s=>s.SubjectId==flashCard.SubjectId), "Id", "Name", flashCard.ChapterId);
           

            return View(flashCard);
        }


        // GET: FossTech/FlashCards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FlashCards == null)
            {
                return NotFound();
            }

            var flashCard = await _context.FlashCards
                .Include(f => f.Board)
                .Include(f => f.Chapter)
                .Include(f => f.Standard)
                .Include(f => f.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flashCard == null)
            {
                return NotFound();
            }

            return View(flashCard);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FlashCards == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FlashCards'  is null.");
            }
            var flashCard = await _context.FlashCards.FindAsync(id);
            if (flashCard != null)
            {
                _context.FlashCards.Remove(flashCard);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult>GetStandardsByBoardId(int boardId)
        {
            var standards=await _context.Standards.Where(x=>x.BoardId == boardId)
                                                  .Select(s => new
                                                  {
                                                      s.Id,
                                                      s.Name,
                                                  }).ToListAsync();
            return Json(standards);                                                  
        }

        public async Task<IActionResult>GetSubjectsByStandardId(int standardId)
        {
            var subject = await _context.Subjects.Where(x => x.StandardId == standardId)
                                                 .Select(s => new
                                                 {
                                                     s.Id,
                                                     s.Name,
                                                 }).ToListAsync();
            return Json(subject);
        }
        public async Task<IActionResult>GetChaptersBySubjectId(int subjectId)
        {
            var chapter = await _context.Chapters.Where(s => s.SubjectId == subjectId)
                                                .Select(x => new
                                                {
                                                    x.Id,
                                                    x.Name,
                                                }).ToListAsync();
            return Json(chapter);
        }

        private bool FlashCardExists(int id)
        {
            return _context.FlashCards.Any(e => e.Id == id);
        }
    }
}
