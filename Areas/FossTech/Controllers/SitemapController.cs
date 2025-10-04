using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.SEOHelper.Sitemap;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Extensions;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SitemapController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;
        public SitemapController(IWebHostEnvironment env, ApplicationDbContext context)
        {
            _env = env;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateSitemapInRootDirectory()
        {
            var list = new List<SitemapNode>();

            list.Add(new SitemapNode
            {
                LastModified = DateTime.UtcNow,
                Priority = 0.8,
                Url = Url.Action("", "Home", new { Area = "" }, Request.Scheme),
                Frequency = SitemapFrequency.Daily
            });

            list.Add(new SitemapNode
            {
                LastModified = DateTime.UtcNow,
                Priority = 0.8,
                Url = Url.Action("AboutUs", "Home", new { Area = "" }, Request.Scheme),
                Frequency = SitemapFrequency.Daily
            });
            list.Add(new SitemapNode
            {
                LastModified = DateTime.UtcNow,
                Priority = 0.8,
                Url = Url.Action("Contact", "Home", new { Area = "" }, Request.Scheme),
                Frequency = SitemapFrequency.Daily
            });
            

            var products = await _context.Products.ToListAsync();

            foreach (var product in products)
            {
                list.Add(new SitemapNode
                {
                    LastModified = DateTime.UtcNow,
                    Priority = 0.8,
                    Url = Url.Action("Details", "Products", new { Id = product.Id, slug = product.Slug }, Request.Scheme),
                    Frequency = SitemapFrequency.Daily
                });
            }
            
            var cats = await _context.Categories.ToListAsync();

            foreach (var cat in cats)
            {
                list.Add(new SitemapNode
                {
                    LastModified = DateTime.UtcNow,
                    Priority = 0.8,
                    Url = Url.Action("Index", "Products", new { category = cat.Name }, Request.Scheme),
                    Frequency = SitemapFrequency.Daily
                });
            }
            
            var subcats = await _context.Categories.ToListAsync();

            foreach (var cat in subcats)
            {
                list.Add(new SitemapNode
                {
                    LastModified = DateTime.UtcNow,
                    Priority = 0.8,
                    Url = Url.Action("Index", "Products", new { subCategory = cat.Name }, Request.Scheme),
                    Frequency = SitemapFrequency.Daily
                });
            }

            var pages = await _context.Pages.ToListAsync();

            foreach (var page in pages)
            {
                list.Add(new SitemapNode
                {
                    LastModified = DateTime.UtcNow,
                    Priority = 0.8,
                    Url = Url.Action("Details", "Page", new { slug = page.Slug }, Request.Scheme),
                    Frequency = SitemapFrequency.Daily
                });
            }

            new SitemapDocument().CreateSitemapXML(list, _env.ContentRootPath);

            return RedirectToAction(nameof(Index)).WithSuccess("File generated", "File generated");
        }

    }
}
