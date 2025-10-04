using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FossTech.Areas.Dashboard.ViewModels;
using FossTech.Configurations;
using FossTech.Data;
using FossTech.Helpers;
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
    [Authorize(Roles = "User")]
    public class StudyMaterialsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PhonePeService _phonePe;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailService _emailService;
        private readonly IEmailConfiguration _emailConfiguration;
        public StudyMaterialsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, PhonePeService pp, IWebHostEnvironment webHostEnvironment, IEmailService emailService, IEmailConfiguration emailConfiguration)
        {
            _context = context;
            _userManager = userManager;
            _phonePe = pp;
            _webHostEnvironment = webHostEnvironment;
            _emailService = emailService;
            _emailConfiguration = emailConfiguration;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.User = user;
            ViewBag.Showall = false;

            var businessProfile = await _context.BusinessProfile.FirstOrDefaultAsync();

            var studyMaterialsQuery = _context.StudyMaterials.AsQueryable();

            if (user.isStudent)
            {
                ViewBag.Showall = true;
            }

            studyMaterialsQuery = studyMaterialsQuery
                .Where(sm => sm.BoardId == user.BoardId && sm.StandardId == user.StandardId)
                .Include(sm => sm.Board)
                .Include(sm => sm.PDFs)
                .Include(sm => sm.Standard)
                .Include(sm => sm.Subject)
                .Include(sm => sm.Chapter);

            var studyMaterials = await studyMaterialsQuery.ToListAsync();

            var unlockedIds = await _context.UserStudyMaterialAccesses
                .Where(a => a.User.Id == user.Id && a.PaymentStatus == "SUCCESS")
                .Select(a => a.StudyMaterialId)
                .ToListAsync();

            ViewBag.UnlockedStudyMaterialIds = unlockedIds;

            return View(studyMaterials);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StudyMaterials == null)
            {
                return NotFound();
            }

            var studyMaterial = await _context.StudyMaterials
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studyMaterial == null)
            {
                return NotFound();
            }

            return View(studyMaterial);
        }

        public IActionResult ViewPdf(int id)
        {
            var studyMaterial = _context.StudyMaterials
                .Include(s => s.Product)
                .Include(x => x.PDFs)
                .FirstOrDefault(s => s.Id == id);

            if (studyMaterial == null)
            {
                return NotFound();
            }

            return View(studyMaterial);
        }
        [HttpPost]
        public async Task<IActionResult> SendEnquiry([FromBody] StudyMaterialEnquiry enquiry)
        {
            if (enquiry == null)
            {
                return BadRequest("Invalid enquiry data.");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            enquiry.UserName = user.UserName;
            enquiry.Name = user.FirstName + " " + user.LastName;
            enquiry.MobileNumber = user.PhoneNumber ?? "N/A";
            enquiry.CreatedAt = DateTime.UtcNow;

            _context.studyMaterialEnquiries.Add(enquiry);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Enquiry sent successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> InitiatePayment([FromBody] PaymentRequestModel req)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return BadRequest();

            var studeyMaterial = await _context.StudyMaterials
                .Include(sm => sm.Product)
                .FirstOrDefaultAsync(sm => sm.Id == req.Id);

            if (studeyMaterial == null) {   
                return NotFound("Study material not found.");
            }

            string orderId = $"{Guid.NewGuid():N}".Substring(0, 15);


            var userStudyMaterialAccess = new UserStudyMaterialAccess
            {
                UserId = user.Id,
                StudyMaterialId = req.Id,
                OrderId = orderId,
                PaymentStatus = "PENDING",
                PurchasedAt = DateTime.Now
            };

            _context.UserStudyMaterialAccesses.Add(userStudyMaterialAccess);
            await _context.SaveChangesAsync();


            string redirectTo = await _phonePe.CreateStudyMaterialPaymentRedirectUrl(studeyMaterial,
                orderId, user.Id, ((double)studeyMaterial.FinalPrice * 100)
            );


            return Ok(new { redirectTo });
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentCallback([FromBody] CheckoutOrderCompletedDto response)
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            var logDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "Logs");
            var logFilePath = Path.Combine(logDirectory, "PaymentCallbackLog.txt");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            await System.IO.File.AppendAllTextAsync(logFilePath, $"{DateTime.Now}: {body}\n\n");
            
            if (response.Type == "CHECKOUT_ORDER_COMPLETED")
            {
                var usma = await _context.UserStudyMaterialAccesses
                    .Include(x => x.User)
                    .Include(x => x.StudyMaterial)
                    .FirstOrDefaultAsync(x => x.PhonePayOrderId == response.Payload.OrderId);
                
                if (usma != null)
                {
                    usma.PaymentStatus = "SUCCESS";
                    _context.Update(usma);
                    await _context.SaveChangesAsync();

                    await SendPaymentSuccessEmail(usma.User, usma.StudyMaterial, "Study Material", usma.OrderId);
                }
                else
                {
                    var ufma = await _context.UserFlashCardAccessess
                        .Include(x => x.User)
                        .Include(x => x.FlashCard)
                        .FirstOrDefaultAsync(x => x.PhonePayOrderId == response.Payload.OrderId);
                    
                    if (ufma != null)
                    {
                        ufma.PaymentStatus = "SUCCESS";
                        _context.Update(ufma);
                        await _context.SaveChangesAsync();

                        await SendPaymentSuccessEmail(ufma.User, ufma.FlashCard, "Flash Card", ufma.OrderId);
                    }
                }
            }

            return Ok(response);
        }

        private async Task SendPaymentSuccessEmail(ApplicationUser user, object content, string contentType, string orderId)
        {
            try
            {
                string contentName = "";
                string contentPrice = "";
                string contentDetails = "";

                if (contentType == "Study Material")
                {
                    var studyMaterial = content as StudyMaterial;
                    contentName = studyMaterial?.Name ?? "Study Material";
                    contentPrice = studyMaterial?.FinalPrice?.ToString("N2") ?? "0.00";
                    contentDetails = $"Board: {studyMaterial?.Board?.Name}, Subject: {studyMaterial?.Subject?.Name}, Standard: {studyMaterial?.Standard?.Name}";
                }
                else if (contentType == "Flash Card")
                {
                    var flashCard = content as FlashCard;
                    contentName = flashCard?.Name ?? "Flash Card";
                    contentPrice = flashCard?.FinalPrice?.ToString("N2") ?? "0.00";
                    contentDetails = $"Board: {flashCard?.Board?.Name}, Subject: {flashCard?.Subject?.Name}, Standard: {flashCard?.Standard?.Name}";
                }

                var emailContent = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0;'>
                            <h2 style='margin: 0;'>Payment Successful!</h2>
                            <p style='margin: 10px 0 0 0;'>Thank you for your purchase</p>
                        </div>
                        
                        <div style='background: #f8f9fa; padding: 20px; border-radius: 0 0 10px 10px;'>
                            <h3 style='color: #2c3e50; margin-top: 0;'>Hello {user.FirstName} {user.LastName},</h3>
                            
                            <p style='color: #555; line-height: 1.6;'>
                                Your payment has been processed successfully. Here are the details of your purchase:
                            </p>
                            
                            <div style='background: white; padding: 15px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #27ae60;'>
                                <h4 style='color: #2c3e50; margin-top: 0;'>{contentName}</h4>
                                <p style='color: #555; margin: 5px 0;'><strong>Type:</strong> {contentType}</p>
                                <p style='color: #555; margin: 5px 0;'><strong>Price:</strong> ₹{contentPrice}</p>
                                <p style='color: #555; margin: 5px 0;'><strong>Order ID:</strong> {orderId}</p>
                                <p style='color: #555; margin: 5px 0;'><strong>Details:</strong> {contentDetails}</p>
                                <p style='color: #555; margin: 5px 0;'><strong>Date:</strong> {DateTime.Now.ToString("MMM dd, yyyy 'at' hh:mm tt")}</p>
                            </div>
                            
                            <p style='color: #555; line-height: 1.6;'>
                                You can now access your purchased content from your dashboard. If you have any questions or need assistance, 
                                please don't hesitate to contact our support team.
                            </p>
                            
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{Url.Action("Index", "Home", new { area = "Dashboard" }, Request.Scheme, Request.Host.Value)}' 
                                   style='background: #27ae60; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                                    Go to Dashboard
                                </a>
                            </div>
                            
                            <div style='border-top: 1px solid #eee; padding-top: 20px; margin-top: 20px;'>
                                <p style='color: #7f8c8d; font-size: 14px; margin: 0;'>
                                    Thank you for choosing our platform for your educational needs!
                                </p>
                                <p style='color: #7f8c8d; font-size: 14px; margin: 5px 0 0 0;'>
                                    Best regards,<br>
                                     VedaEdTech
                                </p>
                            </div>
                        </div>
                    </div>";

                EmailService.EmailMessage emailMessage = new EmailService.EmailMessage
                {
                    FromAddress = new EmailService.EmailAddress
                    {
                        Name = "VedaEdTech",
                        Address = _emailConfiguration.SmtpUsername
                    },
                    ToAddress = new EmailService.EmailAddress
                    {
                        Name = $"{user.FirstName} {user.LastName}",
                        Address = user.Email
                    },
                    Subject = $"Payment Successful - {contentName}",
                    Content = emailContent
                };

                _emailService.Send(emailMessage);
            }
            catch (Exception ex)
            {
                var logDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "Logs");
                var emailLogFilePath = Path.Combine(logDirectory, "EmailErrorLog.txt");
                
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
                
                await System.IO.File.AppendAllTextAsync(emailLogFilePath, 
                    $"{DateTime.Now}: Email sending failed for user {user.Email}, Order {orderId}. Error: {ex.Message}\n\n");
            }
        }

        public async Task<IActionResult> PaymentResult(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            bool unlocked = await _context.UserStudyMaterialAccesses.AnyAsync(a => a.User.Id == user.Id && a.StudyMaterialId == id && a.PaymentStatus == "SUCCESS");
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

        private bool StudyMaterialExists(int id)
        {
            return _context.StudyMaterials.Any(e => e.Id == id);
        }
    }
}
