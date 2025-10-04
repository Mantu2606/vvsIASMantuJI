using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FossTech.Areas.Dashboard.ViewModels;
using FossTech.Data;
using FossTech.Models;
using FossTech.Models.StudentModels;
using FossTech.Models.StudyMaterialModels;
using FossTech.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FossTech.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize(Roles = "User,OutSideUser")]
    public class FlashCardsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PhonePeService _phonePe;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FlashCardsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, PhonePeService phonePe, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _phonePe = phonePe;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }
            ViewBag.User = user;
            ViewBag.ShowAll = false;
            var businessProfile = await _context.BusinessProfile.FirstOrDefaultAsync();
            var flashCard = _context.FlashCards.AsQueryable();
            if (user.isStudent)
            {
                ViewBag.ShowAll = true;
            }
            flashCard = flashCard
                  .Where(sm => sm.BoardId == user.BoardId && sm.StandardId == user.StandardId);
            flashCard = flashCard
                        .Include(f => f.Board)
                        .Include(f => f.Chapter)
                        .Include(f => f.Standard)
                        .Include(f => f.Subject);
            var flashCards = await flashCard.ToListAsync();

            var unlockedIds = await _context.UserFlashCardAccessess
                .Where(a => a.User.Id == user.Id && a.PaymentStatus == "SUCCESS")
                .Select(a => a.FlashCardId)
                .ToListAsync();

            ViewBag.UnlockedFlashCardIds = unlockedIds;

            return View(flashCards);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlashCardPaymentRedirectUrl([FromBody] PaymentRequestModel req)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return BadRequest();

            var flashCard = await _context.FlashCards
                .FirstOrDefaultAsync(sm => sm.Id == req.Id);

            if (flashCard == null)
            {
                return NotFound("flashCard not found.");
            }

            string orderId = $"{Guid.NewGuid():N}".Substring(0, 15);


            var userFlashCardAccess = new UserFlashCardAccess
            {
                UserId = user.Id,
                FlashCardId = req.Id,
                OrderId = orderId,
                PaymentStatus = "PENDING",
                PurchasedAt = DateTime.Now
            };

            _context.UserFlashCardAccessess.Add(userFlashCardAccess);
            await _context.SaveChangesAsync();

            string redirectTo = await _phonePe.CreateFlashCardPaymentRedirectUrl(flashCard,
                orderId, user.Id, ((double)flashCard.FinalPrice * 100)
            );

            return Ok(new { redirectTo });
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentCallback([FromBody] CheckoutOrderCompletedDto response)
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            var logDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "FlashCardLogs");
            var logFilePath = Path.Combine(logDirectory, "PaymentCallbackLog.txt");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            await System.IO.File.AppendAllTextAsync(logFilePath, $"{DateTime.Now}: {body}\n\n");
            if (response.Type == "CHECKOUT_ORDER_COMPLETED")
            {
                var usma = await _context.UserFlashCardAccessess.FirstOrDefaultAsync(x => x.PhonePayOrderId == response.Payload.OrderId);
                usma.PaymentStatus = "SUCCESS";
                _context.Update(usma);
                await _context.SaveChangesAsync();
            }

            return Ok(response);
        }

        public async Task<IActionResult> PaymentResult(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            bool unlocked = await _context.UserFlashCardAccessess.AnyAsync(a => a.User.Id == user.Id && a.FlashCardId == id && a.PaymentStatus == "SUCCESS");
            TempData[$"Unlocked_{id}"] = unlocked;
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public IActionResult ThankYou()
        {
            return View();
        }

        public class PaymentRequestModel
        {
            public int Id { get; set; }
        }

        private bool FlashCardExists(int id)
        {
            return _context.FlashCards.Any(e => e.Id == id);
        }
    }
}
