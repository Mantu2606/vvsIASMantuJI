using System.Linq;
using System.Threading.Tasks;
using FossTech.Data;
using FossTech.Extensions;
using FossTech.Helpers;
using FossTech.Models;
using FossTech.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using FossTech.Models.StudyMaterialModels;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace FossTech.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IEmailConfiguration _emailConfiguration;
        private ITemplateHelper _templateHelper;
        private readonly ApplicationDbContext _context;
        private readonly IMySmsShopConfiguration _mySmsShopConfiguration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDistributedCache _cache;
        private readonly string _secretKey = "6Lcl-bUqAAAAAN5fhWMy7fC5nCjkDkr2qY0cVIrV";


        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IEmailService emailService, IEmailConfiguration emailConfiguration, ITemplateHelper templateHelper, ApplicationDbContext context, IMySmsShopConfiguration mySmsShopConfiguration, IWebHostEnvironment webHostEnvironment, IDistributedCache cache)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _emailConfiguration = emailConfiguration;
            _templateHelper = templateHelper;
            _context = context;
            _mySmsShopConfiguration = mySmsShopConfiguration;
            _webHostEnvironment = webHostEnvironment;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Index(UpdateProfile profile)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                user.Email = profile.Email;
                user.FirstName = profile.FirstName;
                user.LastName = profile.LastName;
                user.UserName = profile.Email;
                user.PhoneNumber = profile.PhoneNumber;

                await _userManager.UpdateAsync(user);

                return View().WithSuccess("Profile has been updated.", null);
            }

            return View().WithWarning("Please fill correct details.", null);
        }


        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [TempData]
        public string ErrorMessage { get; set; }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "~/", string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(ExternalLogin));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(ExternalLogin));
            }
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Dashboard");
                //_logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                //return RedirectToAction(nameof(returnUrl));
            }
            else
            {
                string email = string.Empty;
                string firstName = string.Empty;
                string lastName = string.Empty;
                string profileImage = string.Empty;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    email = info.Principal.FindFirstValue(ClaimTypes.Email);
                }
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
                {
                    firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                }
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
                {
                    lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
                }
                if (info.Principal.HasClaim(c => c.Type == "picture"))
                {
                    profileImage = info.Principal.FindFirstValue("picture");
                }
                var olduser = await _userManager.FindByEmailAsync(email: email);
                if (olduser != null)
                {
                    var roles = await _userManager.GetRolesAsync(olduser);
                    if (!roles.Any())
                    {
                        await _userManager.AddToRoleAsync(olduser, "User");
                    }
                    await _signInManager.SignInAsync(olduser, isPersistent: true);
                    return LocalRedirect(returnUrl);

                }

                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true,
                    isStudent = false,
                    ProfilePhoto=profileImage,
                };
                var result2 = await _userManager.CreateAsync(user);
                if (result2.Succeeded)
                {
                    result2 = await _userManager.AddLoginAsync(user, info);
                    if (result2.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        if (!roles.Any())
                        {
                            await _userManager.AddToRoleAsync(user, "User");
                        }
                        await _signInManager.SignInAsync(user, isPersistent: true);
                        return LocalRedirect(returnUrl);
                    }
                }
                return View("Login");
            }
        }



        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UpdateProfile profile)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                user.Email = profile.Email;
                user.FirstName = profile.FirstName;
                user.LastName = profile.LastName;
                user.UserName = profile.Email;
                user.PhoneNumber = profile.PhoneNumber;
                await _userManager.UpdateAsync(user);

                return View().WithSuccess("Profile has been updated.", null);
            }

            return View().WithWarning("Please fill correct details.", null);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult LoginWithOtp(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult OTP(string PhoneNumber, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            var model = new Otp
            {
                PhoneNumber = PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> OTP(Otp model, string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.PhoneNumber);

                var status = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider, model.Code);
                if (status)
                {
                    await _signInManager.SignInAsync(user, true);
                    if (returnUrl == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    return LocalRedirect(returnUrl);
                }
                ModelState.AddModelError(string.Empty, "Invalid OTP.");
            }

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithOtp(LoginWithOtp model, string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.PhoneNumber);

                if (user != null)
                {
                    try
                    {
                        var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);

                        var username = _mySmsShopConfiguration.Username;
                        var password = _mySmsShopConfiguration.Password;
                        var senderid = _mySmsShopConfiguration.SenderId;

                        var values = new Dictionary<string, string>();

                        values.Add("username", username);
                        values.Add("password", password);
                        values.Add("senderid", senderid);
                        values.Add("route", "1");
                        values.Add("number", model.PhoneNumber.Replace("+91", ""));


                        values.Add("message", Utility.SMSTemplates
                            .OTP
                            .Replace("{#var#}", code));

                        var newUrl = new Uri(QueryHelpers.AddQueryString(_mySmsShopConfiguration.BaseURL + "/http-api.php", values));


                        var httpClient = new HttpClient();
                        var response = await httpClient.GetAsync(newUrl);

                        var content = await response.Content.ReadAsStringAsync();

                        ViewBag.ReturnUrl = returnUrl;
                        //return Json(content);
                        return RedirectToAction(nameof(Otp), new { PhoneNumber = model.PhoneNumber, returnUrl = returnUrl });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Account Not found with this number.");
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp([FromBody] LoginWithOtp model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.PhoneNumber);

                if (user != null)
                {
                    try
                    {
                        var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);

                        var username = _mySmsShopConfiguration.Username;
                        var password = _mySmsShopConfiguration.Password;
                        var senderid = _mySmsShopConfiguration.SenderId;

                        var values = new Dictionary<string, string>();

                        values.Add("username", username);
                        values.Add("password", password);
                        values.Add("senderid", senderid);
                        values.Add("route", "1");
                        values.Add("number", model.PhoneNumber.Replace("+91", ""));


                        values.Add("message", Utility.SMSTemplates
                            .OTP
                            .Replace("{#var#}", code));

                        var newUrl = new Uri(QueryHelpers.AddQueryString(_mySmsShopConfiguration.BaseURL + "/http-api.php", values));


                        var httpClient = new HttpClient();
                        var response = await httpClient.GetAsync(newUrl);

                        var content = await response.Content.ReadAsStringAsync();

                        return Json(true);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Account Not found with this number.");
                }
            }


            return BadRequest();
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;

            var businessProfile = await _context.BusinessProfile.FirstOrDefaultAsync();
            var logo = await _context.Logos.FirstOrDefaultAsync();

            var model = new Login
            {
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
                BusinessProfile = businessProfile,
                Logo = logo
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Admin") || roles.Contains("SuperAdmin"))
                    {
                        return RedirectToAction("Index", "FossTech");
                    }
                    else if (roles.Contains("User") || roles.Contains("OutSideUser"))
                    {
                        return RedirectToAction("Home", "Dashboard");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            ViewBag.ReturnUrl = returnUrl;

            var businessProfile = await _context.BusinessProfile.FirstOrDefaultAsync();
            var logo = await _context.Logos.FirstOrDefaultAsync();
            model.BusinessProfile = businessProfile;
            model.Logo = logo;
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home").WithSuccess("You are now logged Out.", null);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home").WithSuccess("", "Already Logged In.");
            }
            //var branches = await _context.Branches?.ToListAsync() ?? new List<Branch>(); 
            var board = await _context.Boards?.ToListAsync() ?? new List<Board>();
            var standard = await _context.Standards?.ToListAsync() ?? new List<Standard>();


            //ViewData["Branches"] = new SelectList(branches, "Id", "BranchName");
            //ViewData["Products"] = new MultiSelectList(products, "Id", "Name");
            ViewData["Board"] = new SelectList(board, "Id", "Name");
            ViewData["Standard"] = new SelectList(standard, "Id", "Name");

            var model = new Register()
            {
                BusinessProfile = await _context.BusinessProfile.FirstOrDefaultAsync(),
                Logo = await _context.Logos.FirstOrDefaultAsync(),
            };

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.Honeypot))
                {
                    await LogBotAttempt("Honeypot triggered");
                    ModelState.AddModelError(string.Empty,"Registration failed.");
                    return View(model);
                }

                var ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

                if (await IsRateLimited(ipAddress))
                {
                    await LogBotAttempt("Too many registration attempts");
                    ModelState.AddModelError(string.Empty, "Too many registration attempts from this IP. Please try again later.");
                    return View(model);
                }

                var recaptchaResponse = Request.Form["g-recaptcha-response"];
                if (!await VerifyRecaptcha(recaptchaResponse))
                {
                    ModelState.AddModelError(string.Empty, "reCAPTCHA verification failed. Please try again.");
                }

                var board = await _context.Boards.FindAsync(model.BoardId);
                var standard = await _context.Standards.FindAsync(model.StandardId);

                if (board == null || standard == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid selection for Board, Standard, or Branch.");
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.MobileNumber,
                    Board = board,
                    Standard = standard,
                    isStudent = false,
                    CreatedAt= DateTime.Now,
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var defaultRole = "User";
                    var roleResult = await _userManager.AddToRoleAsync(user, defaultRole);
                    if (!roleResult.Succeeded)
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    //return LocalRedirect(returnUrl).WithSuccess("Your Account has been registered. Welcome, <strong>" + model.FirstName + "</strong>", null);
                    return RedirectToAction("Index", "Dashboard").WithSuccess("Your account has been registered. Welcome, <strong>" + model.FirstName + "</strong>", null);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewData["Board"] = new SelectList(await _context.Boards?.ToListAsync() ?? new List<Board>(), "Id", "Name");
            ViewData["Standard"] = new SelectList(await _context.Standards?.ToListAsync() ?? new List<Standard>(), "Id", "Name");
            return View(model).WithError("Please fill in the correct details.", null);
        }

        private async Task LogBotAttempt(string reason)
        {
            var ipAddress= Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var userAgent= Request.Headers["User-Agent"].ToString();
            var timeStamp = DateTime.UtcNow;
            var botAttempt = new BotAttempt
            {
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Reason = reason,
                AttemptedAt = timeStamp,
            };
            _context.BotAttempts.Add(botAttempt);
            await _context.SaveChangesAsync();
        }

        private async Task<bool> IsRateLimited(string ipAddress)
        {
            var cacheKey = $"register_attempts_{ipAddress}";
            var attempts = await _cache.GetStringAsync(cacheKey);
            if (attempts != null && int.Parse(attempts) > 2)
            {
                return true;
            }
            await _cache.SetStringAsync(cacheKey, (int.Parse(attempts ?? "0") + 1).ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });

            return false;
        }
        private async Task<bool> VerifyRecaptcha(string recaptchaResponse)
        {
            using (var client = new HttpClient())
            {
                var verificationUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={_secretKey}&response={recaptchaResponse}";
                var verificationResponse = await client.PostAsync(verificationUrl, null);
                var responseContent = await verificationResponse.Content.ReadAsStringAsync();

                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                return jsonResponse.GetProperty("success").GetBoolean();
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword forgotPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgotPassword.Email);
                if (user == null)
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.Action("ResetPassword", "Account",
                    new { code }, HttpContext.Request.Scheme);


                EmailService.EmailMessage emailMessage = new EmailService.EmailMessage
                {
                    FromAddress = new EmailService.EmailAddress
                    {
                        Name = "Unique Creation",
                        Address = _emailConfiguration.SmtpUsername
                    },
                    ToAddress = new EmailService.EmailAddress
                    {
                        Name = user.FirstName + user.LastName,
                        Address = user.Email
                    },
                    Subject = "Reset your password!",
                    Content = await _templateHelper.GetTemplateHtmlAsStringAsync("Templates/ResetPassword", callbackUrl)
                };

                _emailService.Send(emailMessage);

                return RedirectToAction("ForgotPasswordConfirmation");
            }
            return View();

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }

            var model = new RestPassword
            {
                Code = code
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(RestPassword Input)
        {
            if (!ModelState.IsValid)
            {
                return View(Input);
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(Input);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest();
            }

            var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest();
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return View();
            }
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
