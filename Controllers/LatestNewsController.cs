using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FossTech.Data;
using FossTech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;
using X.PagedList.Extensions;

namespace FossTech.Controllers
{
    public class LatestNewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LatestNewsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Route("latestnews")]
        public async Task<IActionResult> Index(string query = null, int pageindex = 1, int PageSize = 12)
        {
            ViewBag.Posts = await _context.LatestNews.OrderByDescending(a => a.CreatedAt).Take(4).ToListAsync();
            if (query == null)
            {
                return View( _context.LatestNews.OrderByDescending(o => o.LastModified).ToPagedList(pageNumber: pageindex, pageSize: PageSize));
            }

            ViewBag.Query = query;

            var posts = _context.LatestNews.Where(x => x.Title.Contains(query)).OrderByDescending(o => o.LastModified).ToPagedList(pageNumber: pageindex, pageSize: PageSize);
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(posts);
        }

        [Route("latestnews/{Id?}/{slug?}")]
        public async Task<IActionResult> Details(int? id, string slug)
        {
          
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.LatestNews
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            ViewBag.Posts = await _context.LatestNews.OrderByDescending(a => a.CreatedAt).Take(4).ToListAsync();
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(post);
        }

    }
}