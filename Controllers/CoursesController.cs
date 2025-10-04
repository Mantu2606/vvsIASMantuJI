using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FossTech.Data;
using FossTech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Models.ProductModels;
using X.PagedList;
using Z.EntityFramework.Plus;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using FossTech.Extensions;

namespace FossTech.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CoursesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager = null)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string brand, string subCategory, string query, int minPrice, int maxPrice, string category, string orderby = null)
        {
           

            IQueryable<Product> products = from s in _context.Products
                                           select s;
            products = products.Include(a => a.Images);
            products = products.Include(x => x.Reviews);

            var sections = await _context.Sections.OrderBy(x=>x.SortOrder).ToListAsync();
            var categories = await _context.Categories.Take(8).ToListAsync();
            var random = new Random();
            ViewBag.Category = categories.OrderBy(x => random.Next()).ToList();
            ViewBag.BusinessProfile = await _context.BusinessProfile.FirstOrDefaultAsync();
            ViewBag.Sections = sections;



            if (category != null && category.Trim().ToLower() != "all")
            {
                var category1 = await _context.Categories.Where(c => c.Slug == category).FirstOrDefaultAsync();

                if (category1 != null)
                {
                    products = products.Include(x => x.ProductCategories).Where(x => x.ProductCategories.Any(x => x.CategoryId == category1.Id));
                }
            }

            if (subCategory != null)
            {
                var subCategory1 = await _context.SubCategories.Where(c => c.Name == subCategory).FirstOrDefaultAsync();

                if (subCategory1 != null)
                {
                    products = products.Include(x => x.ProductSubCategories)
                    .Where(x => x.ProductSubCategories.Any(x => x.SubCategoryId == subCategory1.Id));
                }

            }

            if (query != null)
            {
                products = products.Where(s => s.Name.Contains(query));
                
            }

            switch (orderby)
            {
                case "Date":
                    products = products.OrderBy(s => s.CreatedAt);
                    break;
                case "price":
                    products = products.OrderBy(s => s.BasePrice);
                    break;
                case "price-desc":
                    products = products.OrderByDescending(s => s.BasePrice);
                    break;

                case "latest-collection":
                    products = products.OrderByDescending(a => a.CreatedAt);
                    break;
                default:
                    products = products.OrderBy(s => s.SortOrder);
                    break;
            }

            if (minPrice > 0)
            {
                products = products.Where(p => p.BasePrice >= minPrice);
                ViewBag.minPrice = minPrice;
            }

            if (maxPrice > 0)
            {
                products = products.Where(p => p.BasePrice <= maxPrice);
                ViewBag.maxPrice = maxPrice;
            }

            ViewBag.OrderByList = new List<SelectListItem>()
            {
                new SelectListItem() { Value="default", Text= "Default" },
                new SelectListItem() { Value="latest-collection", Text= "Latest Collection" },
                new SelectListItem() { Value="sort-by-popularity", Text= "Sort By Popularity" },
                new SelectListItem() { Value="price", Text= "Price (Low to High)" },
                new SelectListItem() { Value="price-desc", Text= "Price (High to Low)" },
            };

            //ViewBag.PageSizes = new List<SelectListItem>()
            //{
            //    new SelectListItem() { Value="8", Text= "8" },
            //    new SelectListItem() { Value="12", Text= "12" },
            //    new SelectListItem() { Value="24", Text= "24" },
            //    new SelectListItem() { Value="48", Text= "48" },
            //    new SelectListItem() { Value="100", Text= "100" },
            //};

           
            var pageSizes = new List<SelectListItem>();

            for (var j = 1; j <= 10; j++)
            {
                pageSizes.Add(new SelectListItem() { Value = Convert.ToString(40 * j), Text = Convert.ToString(40 * j) });
            }

            ViewBag.PageSizes = pageSizes;
            ViewBag.categoryName = category;
            ViewBag.orderby = orderby;
            ViewBag.query = query;
            ViewBag.subCategory = subCategory;
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(
                await products.Where(x => x.Status)
                    .ToListAsync());
        }

       


        [Route("{controller}/{Action}")]
        public async Task<IActionResult> Fill()
        {
            if (true)
            {
                _context.RemoveRange(await _context.Products.ToListAsync());
                await _context.SaveChangesAsync();

                var JSONString = await System.IO.File.ReadAllTextAsync("Products.json");

                var products = JsonSerializer.Deserialize<List<Product>>(JSONString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                //return Json(products.Count);
                if (products != null)
                {

                    foreach (var product in products)
                    {
                        await Action(product);
                    }
                }
            }
            return Json(await _context.Products.CountAsync());
        }

        private async Task Action(Product p)
        {
            Product product = new()
            {
                Name = p.Name,
                Image = p.Image,
                Description = p.Description,
                ShortDescription = p.ShortDescription,
                BasePrice = p.BasePrice,
                DiscountAmount = p.DiscountAmount,
                ProductTags = p.ProductTags,
                Status = p.Status
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            if (p.Images.Any())
            {
                foreach (var image in p.Images)
                {
                    ProductImage productImage = new ProductImage { ProductId = product.Id, Order = image.Order, Img = image.Img };
                    await _context.AddAsync(productImage);
                }

                await _context.SaveChangesAsync();
            }
            
        }

        [Route("{controller}/{Action}/{id}")]
        public async Task<IActionResult> GetProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Products
                .Where(x => x.Status == true)
                .Include(x => x.Reviews)
                .ThenInclude(x => x.User)
                .Include(a => a.ProductCategories)
                .ThenInclude(a => a.Category)
                .Include(c => c.Images)
                .Where(m => m.Id == id).FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return Json(product);
        }

        
        [Route("{controller}/{id}/{slug?}")]
        public async Task<IActionResult> Details(int id, string slug)
        {

            if (id == 0)
            {
                return NotFound();
            }
            var product = await _context.Products
                .Where(x => x.Status == true)
                .Include(x => x.Reviews)
                .ThenInclude(x => x.User)
                .Include(a => a.ProductCategories)
                .ThenInclude(a => a.Category)
                //.Include(m => m.Category)
                //.Include(m => m.SubCategory)
                .Include(c => c.Images)
                .Include(x => x.ProdutFaq)
                .Where(m => m.Id == id).FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }
            var items = await _context.Products
                .Where(x => x.Status)
                .Take(4)
                .Include(x => x.Reviews)
                .ToListAsync();

            ViewBag.items = items;

            ViewBag.ProductId = id;

            ViewBag.RelatedProducts = await _context.Products.OrderBy(x => Guid.NewGuid()).Take(4).ToListAsync();
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            /*ViewBag.Products = await _context.Products.ToListAsync();*/

            return View(product);
        }


        [HttpPost]
        [Route("{controller}/{Action}")]
        public async Task<IActionResult> CoursesEnquiryForm(CoursesEnquiry depositAndLoanEnquiry)
        {

            var productdetail = await _context.Products.Where(x => x.Id == depositAndLoanEnquiry.ProductId).FirstOrDefaultAsync();
            if (ModelState.IsValid)
            {
                _context.Add(depositAndLoanEnquiry);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details" , "Courses" , new { id = productdetail.Id , slug = productdetail.Slug }).WithSuccess("Your Form Has Submitted Successfully We Will Call You Shortly", null);
            }
            return View(depositAndLoanEnquiry);
        }


        //public async Task<IActionResult> GetVariant(VariantViewModel model)
        //{
        //    var variant = await _context.VariantOptions.Where(x => x.OptionId == model.ProductId).Where(x => x.OptionId == model.OptionId)
        //        .Where(x => x.OptionValueId == model.valu)
        //    return Ok(variant);
        //}

        [Route("Show/{Id?}/{menuName?}")]
        public async Task<IActionResult> Show(int? id, string menuName)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus.Include(a => a.MenuProducts).ThenInclude(a => a.Product).FirstOrDefaultAsync(a => a.Id == id);
            if (menu != null)
            {
                var products = menu.MenuProducts.Where(a => a.MenuId == menu.Id).Select(a => a.Product).ToList();
                return View(products);
            }

            return NotFound();
        }

        [Route("{controller}/{action}/{productId}")]
        public async Task<IActionResult> QuickVIew(int? productId)
        {
            
            if (productId == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Where(x => x.Status == true)
                .Include(a => a.ProductCategories)
                .ThenInclude(a => a.Category)
                //.Include(m => m.Category)
                //.Include(m => m.SubCategory)
                .Include(c => c.Images)
                .FirstOrDefaultAsync(x => x.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

       
        public class VariantViewModel
        {
            public int ProductId { get; set; }
            public int OptionId { get; set; }
            public int OptionValueId { get; set; }
        }
    }
}