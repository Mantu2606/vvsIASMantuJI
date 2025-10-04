using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models.StudentModels;
using Microsoft.AspNetCore.Identity;
using FossTech.Models;
using FossTech.Areas.FossTech.ViewModels;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PurchaseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? boardId, int? standardId, int? subjectId, string paymentStatus)
        {
            if (string.IsNullOrEmpty(paymentStatus))
            {
                paymentStatus = "SUCCESS";
            }

            var boards = await _context.Boards.OrderBy(b => b.Name).ToListAsync();
            var standards = await _context.Standards.OrderBy(s => s.Name).ToListAsync();
            var subjects = await _context.Subjects.OrderBy(su => su.Name).ToListAsync();
            var paymentStatuses = new List<string> { "SUCCESS", "PENDING", "Failed" };

            var userStudyMaterialAccessesQuery = _context.UserStudyMaterialAccesses
                .Include(u => u.StudyMaterial)
                .ThenInclude(x => x.Board)
                .Include(u => u.StudyMaterial)
                .ThenInclude(x => x.Subject)
                .Include(u => u.StudyMaterial)
                .ThenInclude(x => x.Standard)
                .Include(u => u.User)
                .AsQueryable();

            if (boardId.HasValue)
                userStudyMaterialAccessesQuery = userStudyMaterialAccessesQuery.Where(u => u.StudyMaterial.BoardId == boardId);
            if (standardId.HasValue)
                userStudyMaterialAccessesQuery = userStudyMaterialAccessesQuery.Where(u => u.StudyMaterial.StandardId == standardId);
            if (subjectId.HasValue)
                userStudyMaterialAccessesQuery = userStudyMaterialAccessesQuery.Where(u => u.StudyMaterial.SubjectId == subjectId);
            if (!string.IsNullOrEmpty(paymentStatus))
                userStudyMaterialAccessesQuery = userStudyMaterialAccessesQuery.Where(u => u.PaymentStatus == paymentStatus);

            var userStudyMaterialAccesses = await userStudyMaterialAccessesQuery
                .OrderByDescending(u => u.PurchasedAt)
                .ToListAsync();

            var userFlashCardAccessesQuery = _context.UserFlashCardAccessess
                .Include(u => u.FlashCard)
                .ThenInclude(x => x.Board)
                .Include(u => u.FlashCard)
                .ThenInclude(x => x.Subject)
                .Include(u => u.FlashCard)
                .ThenInclude(x => x.Standard)
                .Include(u => u.User)
                .AsQueryable();

            if (boardId.HasValue)
                userFlashCardAccessesQuery = userFlashCardAccessesQuery.Where(u => u.FlashCard.BoardId == boardId);
            if (standardId.HasValue)
                userFlashCardAccessesQuery = userFlashCardAccessesQuery.Where(u => u.FlashCard.StandardId == standardId);
            if (subjectId.HasValue)
                userFlashCardAccessesQuery = userFlashCardAccessesQuery.Where(u => u.FlashCard.SubjectId == subjectId);
            if (!string.IsNullOrEmpty(paymentStatus))
                userFlashCardAccessesQuery = userFlashCardAccessesQuery.Where(u => u.PaymentStatus == paymentStatus);

            var userFlashCardAccesses = await userFlashCardAccessesQuery
                .OrderByDescending(u => u.PurchasedAt)
                .ToListAsync();

            var pendingStudyMaterials = userStudyMaterialAccesses.Where(u => u.PaymentStatus == "PENDING").ToList();
            var pendingFlashCards = userFlashCardAccesses.Where(u => u.PaymentStatus == "PENDING").ToList();

            var viewModel = new AdminPurchaseHistoryViewModel
            {
                StudyMaterialAccesses = userStudyMaterialAccesses,
                FlashCardAccesses = userFlashCardAccesses,
                PendingStudyMaterials = pendingStudyMaterials,
                PendingFlashCards = pendingFlashCards,
                Boards = boards,
                Standards = standards,
                Subjects = subjects,
                PaymentStatuses = paymentStatuses,
                SelectedBoardId = boardId,
                SelectedStandardId = standardId,
                SelectedSubjectId = subjectId,
                SelectedPaymentStatus = paymentStatus
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePaymentStatus(int id, string type, string status)
        {
            try
            {
                if (type == "studyMaterial")
                {
                    var studyMaterialAccess = await _context.UserStudyMaterialAccesses.FindAsync(id);
                    if (studyMaterialAccess != null)
                    {
                        studyMaterialAccess.PaymentStatus = status;
                        _context.Update(studyMaterialAccess);
                    }
                }
                else if (type == "flashCard")
                {
                    var flashCardAccess = await _context.UserFlashCardAccessess.FindAsync(id);
                    if (flashCardAccess != null)
                    {
                        flashCardAccess.PaymentStatus = status;
                        _context.Update(flashCardAccess);
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Payment status updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating payment status: " + ex.Message });
            }
        }
    }
}