using System.Diagnostics;
using FossTech.Helpers;
using FossTech.Extensions;
using FossTech.Data;
using FossTech.Models;
using System.IO;
using FossTech.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using FossTech.Models.ProductModels;
using System.Text.Json;

namespace FossTech.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IEmailConfiguration _emailConfiguration;
        private readonly ITemplateHelper _templateHelper;
        private readonly UserManager<ApplicationUser> _userManager;     
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _environment;
        private readonly string _secretKey = "6Lcl-bUqAAAAAN5fhWMy7fC5nCjkDkr2qY0cVIrV";


        public HomeController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ITemplateHelper templateHelper, IEmailConfiguration emailConfiguration, IEmailService emailService, ApplicationDbContext context, ILogger<HomeController> logger, IWebHostEnvironment environment)
            
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _templateHelper = templateHelper;
            _emailConfiguration = emailConfiguration;
            _emailService = emailService;
            _context = context;
            _logger = logger;
            _environment = environment;
        }


        public async Task<IActionResult> Index(string s = null)
        {
            var websetting = await _context.WebSettings.OrderBy(x=>x.CreatedAt).FirstOrDefaultAsync();
            if (websetting != null)
            {
                if (websetting.IsUnderConstruction)
                {
                    // Combine the ContentRootPath with the relative path to your HTML file
                    var filePath = System.IO.Path.Combine(_environment.ContentRootPath, "index.html");
                    return PhysicalFile(filePath, "text/html");
                }
            }

            var SectionId = _context.Sections.OrderBy(x=>x.SortOrder).FirstOrDefault(x => x.Slug == s)?.Id;

         
            var about = new AboutUs();
            if (SectionId == null)
            {
                about= await _context.AboutUs.OrderBy(x=>x.SortOrder).FirstOrDefaultAsync();
            }
            else
            { 
                about =  await _context.AboutUs.OrderBy(x=>x.SortOrder).Where(s => s.SectionId == SectionId).FirstOrDefaultAsync();
            }

            var banner= new Banner();
            if (SectionId == null)
            {
                banner= await _context.Banners.OrderBy(x=>x.CreatedAt).FirstOrDefaultAsync();
            }
            else
            {
                banner = await _context.Banners.OrderBy(x=>x.CreatedAt).Where(s => s.SectionId == SectionId).FirstOrDefaultAsync();
            }

            var images = new List<SliderImage>();
            if(SectionId==null)
            {
                images = await _context.SliderImages.OrderBy(x=>x.Id).ToListAsync();
            }
            else
            {
                images = await _context.SliderImages.OrderBy(x=>x.Id).Where(s => s.SectionId == SectionId).ToListAsync();
            }
            var update= new List<Post>();
            if (SectionId == null)
            {
                update = await _context.Posts.OrderByDescending(s => s.Id).Take(4).ToListAsync();
            }
            else
            {
                update = await _context.Posts.Where(s=>s.SectionId==SectionId).OrderByDescending(s => s.Id).Take(4).ToListAsync();
            }

            var gallery = new Image();
            if (SectionId == null)
            {
                gallery = await _context.Images.OrderByDescending(s => s.Id).Take(8).FirstOrDefaultAsync();
            }
            else
            {
                gallery = await _context.Images.Where(s => s.SectionId == SectionId).OrderByDescending(s => s.Id).Take(8).FirstOrDefaultAsync();
            }

            var supdate= new Post();
            if (SectionId == null)
            {
                 supdate = await _context.Posts.OrderBy(x=>x.SortOrder).FirstOrDefaultAsync();
            }
            else
            {
                supdate = await _context.Posts.OrderBy(x=>x.SortOrder).Where(s => s.SectionId == SectionId).FirstOrDefaultAsync();
            }

            var businessProfile= new BusinessProfile();
            if (SectionId==null)
            {
             businessProfile = await _context.BusinessProfile.OrderByDescending(x=>x.Id).FirstOrDefaultAsync();
                
            }
            else
            {
                businessProfile = await _context.BusinessProfile.OrderByDescending(x => x.Id).Where(s => s.SectionId == SectionId).FirstOrDefaultAsync();
            }

            var faqs = new List<FAQ>();
            if (SectionId==null)
            {
                faqs = await _context.FAQs.Take(websetting.FAQsCount).OrderBy(x => x.Order).ToListAsync();                
            }
            else
            {
                faqs = await _context.FAQs.Where(s => s.SectionId == SectionId).Take(websetting.FAQsCount).OrderBy(x => x.Order).ToListAsync();
            }

            var categorylist = new List<Category>();
            if (SectionId==null)
            {
                categorylist = await _context.Categories.OrderBy(x=>x.SortOrder).Take(websetting.CategoryCount).ToListAsync();               
            }
            else
            {
                categorylist = await _context.Categories.OrderBy(x=>x.SortOrder).Where(s => s.SectionId == SectionId).Take(websetting.CategoryCount).ToListAsync();

            }

            var section = await _context.Sections.OrderBy(x=>x.SortOrder).ToListAsync();

            var registrationBanner= new RegistrationBanner();
            if (SectionId==null)
            {
                 registrationBanner = await _context.RegistrationBanners.OrderBy(x=>x.CreatedAt).FirstOrDefaultAsync();

            }
            else
            {
                registrationBanner = await _context.RegistrationBanners.OrderByDescending(x => x.Id).Where(s => s.SectionId == SectionId).FirstOrDefaultAsync();

            }

            var team= new List<Team>();
            if (SectionId==null)
            {
                team = await _context.Teams.OrderBy(x=>x.SortOrder).ToListAsync();
            }
            else
            {
                team = await _context.Teams.OrderBy(x=>x.SortOrder).Where(s => s.SectionId == SectionId).ToListAsync();
            }

            var newUpdate = new List<Models.Update>();
            if (SectionId==null)
            {
                newUpdate = await _context.Updates.OrderBy(x => x.SortOrder).Take(4).ToListAsync();
            }
            else
            {
                newUpdate = await _context.Updates.Where(s => s.SectionId == SectionId).OrderBy(x => x.SortOrder).Take(4).ToListAsync();

            }

            var latestNews = new List<LatestNews>();
            if (SectionId == null)
            {
                 latestNews = await _context.LatestNews.OrderBy(x => x.SortOrder).Take(websetting.LatestNewsCount).ToListAsync();

            }
            else
            {
                 latestNews = await _context.LatestNews.Where(s => s.SectionId == SectionId).OrderBy(x => x.SortOrder).Take(websetting.LatestNewsCount).ToListAsync();

            }
            var toppers = new List<OurTopper>();
            if (SectionId==null)
            {
                 toppers = await _context.OurToppers.OrderBy(x => x.SortOrder).Take(4).ToListAsync();

            }
            else
            {
                 toppers = await _context.OurToppers.Where(s => s.SectionId == SectionId).OrderBy(x => x.SortOrder).Take(4).ToListAsync();

            }

            var placement = new List<Placement>();
            if (SectionId==null)
            {
                 placement = await _context.Placements.OrderBy(x => x.Order).Take(websetting.PlacementCount).ToListAsync();

            }
            else
            {
                placement = await _context.Placements.Where(x=>x.SectionId==SectionId).OrderBy(x => x.Order).Take(websetting.PlacementCount).ToListAsync();
            }

            var OurDirector = new List<OurDirector>();
            if (SectionId==null)
            {
                OurDirector = await _context.OurDirectors.OrderBy(x => x.SortOrder).Take(websetting.DirectorsCount).ToListAsync();
            }
            else
            {
                OurDirector = await _context.OurDirectors.Where(x=>x.SectionId==SectionId).OrderBy(x => x.SortOrder).Take(websetting.DirectorsCount).ToListAsync();

            }

            var OurHOD = new List<OurHOD>();
            if (SectionId==null)
            {
                OurHOD = await _context.OurHODs.OrderBy(x => x.SortOrder).Take(websetting.HODCount).ToListAsync();
            }
            else
            {
                OurHOD = await _context.OurHODs.Where(s => s.SectionId == SectionId).OrderBy(x => x.SortOrder).Take(websetting.HODCount).ToListAsync();
            }

            //var items = new List<Product>();
            //if (SectionId==null)
            //{
            //   items = await _context.Products

            //   .Where(x => x.Status)
            //   .OrderByDescending(s => s.Id)
            //   .Take(websetting.CoursesCount)
            //   .Include(x => x.Reviews)
            //   .Include(x => x.ProductCategories)
            //   .ThenInclude(x => x.Category)
            //   .ToListAsync();
            //}
            //else
            //{
            //    items = await _context.Products
            //  .Where(s=>s.SectionId==SectionId)
            //  .Where(x => x.Status)
            //  .OrderByDescending(s => s.Id)
            //  .Take(websetting.CoursesCount)
            //  .Include(x => x.Reviews)
            //  .Include(x => x.ProductCategories)
            //  .ThenInclude(x => x.Category)
            //  .ToListAsync();
            //}

            var items = new List<Product>();
                if(SectionId==SectionId)
            {
                items=await _context.Products
                    .Where(x=>x.SectionId == SectionId)
                    .Where(x=>x.Status)
                    .OrderBy(x=>x.SortOrder)
                    .Take(websetting.CoursesCount)
                    .Include(x=>x.Reviews)
                    .Include(x=>x.ProductCategories)
                    .ThenInclude(x=>x.Category)
                    .ToListAsync();
            }
            else
            {
                //  items = await _context.Products
                //.Where(s=>s.SectionId==SectionId)
                //.Where(x => x.Status)
                //.OrderByDescending(s => s.Id)
                //.Take(websetting.CoursesCount)
                //.Include(x => x.Reviews)
                //.Include(x => x.ProductCategories)
                //.ThenInclude(x => x.Category)
                //.ToListAsync();
            }

            var testimonials = new List<Testimonial>();
            if (SectionId == null)
            {
                testimonials = await _context.Testimonials.OrderByDescending(x => x.CreatedAt).Take(websetting.TestimonialsCount).ToListAsync();
            }
            else
            {
                testimonials = await _context.Testimonials.Where(s => s.SectionId == SectionId).OrderByDescending(x => x.CreatedAt).Take(websetting.TestimonialsCount).ToListAsync();
            }

            if (websetting == null) 
            {
                websetting = new WebSetting();
            }
            var highlighter = await _context.Highlighters.OrderBy(x=>x.CreatedAt)
                .Where(s => s.SectionId == SectionId)
                .FirstOrDefaultAsync();

            if (highlighter == null)
            {
                highlighter = new Highlighter();
            }

            HomeViewModel model = new HomeViewModel
            {
                SliderImages = images,
                AboutUs = about,
                Updates = update,
                //Image = gallery,
                Sections = section,
                _supdate =supdate,
                RegistrationBanners = registrationBanner,
                BusinessProfile = businessProfile,
                Faqs = faqs,
                _Category = categorylist,
                Testimonials = testimonials,
                _items = items,
                Team = team,

                NewUpdate = newUpdate,
                
                LatestNews = latestNews,

                Toppers = toppers,

                //Placement = Placement,
                WebSetting = websetting,
                Highlighter = highlighter,
                Counters = await _context.Counters.OrderBy(x => x.SortOreder).ToListAsync(),
                OurDirector = OurDirector,

                OurHOD = OurHOD,
            };
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LoadData(int page)
        {
            var items = await _context.Products
                        .Where(x => x.Status)
                        .OrderByDescending(s => s.Id)
                        .Include(x => x.Reviews)
                        .Skip((page - 1) * 15)
                        .Take(15)
                        .ToListAsync();

            // Render the partial view with the new items
            return PartialView("_ItemAreaPartial", items);
        }

        public IActionResult Thankyou()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> AboutUs(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var footer = await _context.AboutUs
                .FirstOrDefaultAsync(m => m.AboutUsId == id);
            if (footer == null)
            {
                return NotFound();
            }

            return View(footer);
        }
       
      
       

        public IActionResult Contact()
        {
            var footer = new BusinessProfile();


          
            if (_context.BusinessProfile.Any())
            {
                footer = _context.BusinessProfile.FirstOrDefault();
            }
            var item = _context.Contacts.FirstOrDefault();
            ContactViewModel model = new ContactViewModel()
            {
                _contact = item,
                BusinessProfile = footer

                
            };
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMessage contactMessage)
        {
            var item = _context.Contacts.FirstOrDefault();
            var footer = _context.BusinessProfile.FirstOrDefault();
            ContactViewModel model = new ContactViewModel()
            {
                _contact = item,
                BusinessProfile = footer
            };
            var recaptchaResponse = Request.Form["g-recaptcha-response"];
            if (!await VerifyRecaptcha(recaptchaResponse))
            {
                ModelState.AddModelError(string.Empty, "reCAPTCHA verification failed. Please try again.");
                return View(model); 
            }
            if (ModelState.IsValid)
            {
                contactMessage.CreatedAt = DateTime.Now;
                _context.Add(contactMessage);
                await _context.SaveChangesAsync();

                if (footer.EmailAddress != null)
                {
                    EmailService.EmailMessage emailMessage = new EmailService.EmailMessage
                    {
                        FromAddress = new EmailService.EmailAddress
                        {
                            Name = _emailConfiguration.Name,
                            Address = _emailConfiguration.SmtpUsername
                        },
                        ToAddress = new EmailService.EmailAddress
                        {
                            Name = footer.BusinessName,
                            Address = footer.EmailAddress
                        },
                        Subject = "You received a Inquiry!",
                        Content = await _templateHelper.GetTemplateHtmlAsStringAsync("Templates/ContactMessage", contactMessage)
                    };

                    _emailService.Send(emailMessage);
                }

                return RedirectToAction("Index").WithSuccess("Your message has been sent.", null); ;
            }


            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");

            return View(model).WithWarning("Please Try again.", null);
        }

        public async Task<bool> VerifyRecaptcha(string recaptchaResponse)
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

        [HttpPost]
        public IActionResult ChangeLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                }
            );

            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> WhatsappClickCounts()
        {
            _context.Add(new WhatsappClickCount() { CreatedAt = DateTime.Now, LastModified = DateTime.Now });
            await _context.SaveChangesAsync();

            return Ok("Success");
        }

        [HttpPost]
        public async Task<IActionResult> CallClickCounts()
        {
            _context.Add(new CallClickCount() { CreatedAt = DateTime.Now, LastModified = DateTime.Now });
            await _context.SaveChangesAsync();

            return Ok("Success");
        }

        public IActionResult Action1()
        {
            return View();
        }

        public IActionResult Action2()
        {
            return View();
        }

        public IActionResult Action3()
        {
            return View();
        }

        public IActionResult Action4()
        {
            return View();
        }
    }

}


    

