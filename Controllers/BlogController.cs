using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FossTech.Data;
using FossTech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace FossTech.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("blog")]
        public async Task<IActionResult> Index(string query = null, int pageindex = 1, int PageSize = 12)
        {
            ViewBag.Posts = await _context.Posts.OrderByDescending(a => a.CreatedAt).Take(4).ToListAsync();
            if (query == null)
            {
                return View( _context.Posts.OrderByDescending(o => o.LastModified).ToPagedList(pageindex, PageSize));
            }

            ViewBag.Query = query;

            var posts =  _context.Posts.Where(x => x.Title.Contains(query)).OrderByDescending(o => o.LastModified).ToPagedList(pageindex, PageSize);
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(posts);
        }

        [Route("blog/{Id?}/{slug?}")]
        public async Task<IActionResult> Details(int? id, string slug)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            ViewBag.Posts = await _context.Posts.OrderByDescending(a => a.CreatedAt).Take(4).ToListAsync();
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View(post);
        }
    }
}