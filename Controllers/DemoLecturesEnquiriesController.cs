using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models;
using FossTech.Extensions;
using Microsoft.AspNetCore.Http;

namespace FossTech.Controllers
{
    public class DemoLecturesEnquiriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DemoLecturesEnquiriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FreeCourses/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("FormSubmittedDate")==DateTime.Now.ToString("yyy-MM-dd"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: FreeCourses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<int> products, DemoLecturesEnquiry freeCourse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(freeCourse);
                await _context.SaveChangesAsync();
                if (products != null)
                {
                    products?.ForEach(c =>
                    {
                        _context.Add(new DemoLectureCoursesList()
                        {
                            DemoLecturesEnquiryId = freeCourse.Id,
                            ProductId = c
                        });
                    });
                    await _context.SaveChangesAsync();
                }
                HttpContext.Session.SetString("FormSubmittedDate", DateTime.Now.ToString("yyyy-MM-dd"));
                freeCourse.IsFoundation = false;
                return RedirectToAction("Index", "Home").WithSuccess("Successful", "Successfully Applied For Free Course Will Call you Shortly");
            }
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name");
            return RedirectToAction("Index", "Home").WithError("Error", "Something Wrong Please try again after some time.");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create1(DemoLecturesEnquiry freeCourse, string products)
        {
            if (ModelState.IsValid)
            {
                _context.Add(freeCourse);
                await _context.SaveChangesAsync();

                if (string.IsNullOrEmpty(products) || products == "foundation")
                {
                    var foundationProduct = _context.Products.FirstOrDefault(p => p.Name == "foundation");
                    if (foundationProduct != null)
                    {
                        _context.Add(new DemoLectureCoursesList()
                        {
                            DemoLecturesEnquiryId = freeCourse.Id,
                            ProductId = foundationProduct.Id
                        });
                        await _context.SaveChangesAsync();
                    }
                }

                freeCourse.IsFoundation = true;  

                return RedirectToAction("Index", "Home").WithSuccess("Successful", "Successfully Applied For Free Course. We will call you shortly.");
            }

            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            return RedirectToAction("Index", "Home").WithError("Error", "Something went wrong. Please try again later.");
        }



        private bool FreeCourseExists(int id)
        {
          return (_context.DemoLecturesEnquiries?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
