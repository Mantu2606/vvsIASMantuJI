using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Data;
using FossTech.Models.Menu;
using DocumentFormat.OpenXml.Wordprocessing;
using System;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class MenusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Menus
        public async Task<IActionResult> Index()
        {
            return View(await _context.Menus.OrderBy(a => a.Order).ToListAsync());
        }

        // GET: Admin/Menus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .Include(a => a.MenuProducts)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // GET: Admin/Menus/Create
        public IActionResult Create()
        {
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["SubCategories"] = new SelectList(_context.SubCategories, "Id", "Name");
            ViewData["Products"] = new SelectList(_context.Products, "Id", "Name");
            ViewData["Pages"] = new SelectList(_context.Pages, "Id", "Name");

            return View();
        }

        // POST: Admin/Menus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<int> products, List<int> categories, List<int> subCategories, List<int> pages, Menu menu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(menu);
                await _context.SaveChangesAsync();

                if (products != null)
                {
                    foreach (var product in products)
                    {
                        await _context.AddAsync(entity: new MenuProduct
                        {
                            ProductId = product,
                            MenuId = menu.Id
                        });
                    }
                }

                if (categories != null)
                {
                    foreach (var category in categories)
                    {
                        await _context.AddAsync(new MenuCategory()
                        {
                            CategoryId = category,
                            MenuId = menu.Id
                        });
                    }
                }

                if (subCategories != null)
                {
                    foreach (var subCategory in subCategories)
                    {
                        await _context.AddAsync(new MenuSubCategory()
                        {
                            SubCategoryId = subCategory,
                            MenuId = menu.Id
                        });
                    }
                }

                if (pages != null)
                {
                    foreach (var page in pages)
                    {
                        await _context.AddAsync(new MenuPage()
                        {
                            PageId = page,
                            MenuId = menu.Id
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name");
            
            ViewData["SubCategories"] = new SelectList(_context.SubCategories, "Id", "Name");

            ViewData["Products"] = new SelectList(_context.Products, "Id", "Name");

            ViewData["Pages"] = new SelectList(_context.Pages, "Id", "Name");

            return View(menu);
        }

        // GET: Admin/Menus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }
            ViewData["Categories"] = new MultiSelectList(_context.Categories, "Id", "Name",
                _context.MenuCategories.Where(a => a.MenuId == id).Select(e => e.CategoryId).ToArray());

            
            ViewData["SubCategories"] = new MultiSelectList(_context.SubCategories, "Id", "Name",
                _context.MenuSubCategories.Where(a => a.MenuId == id).Select(e => e.SubCategoryId).ToArray());


            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name",
                _context.MenuProducts.Where(a => a.MenuId == id).Select(e => e.ProductId).ToArray());

            ViewData["Pages"] = new MultiSelectList(_context.Pages, "Id", "Name",
                _context.MenuPages.Where(a => a.MenuId == id).Select(e => e.PageId).ToArray());

            return View(menu);
        }

        // POST: Admin/Menus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, List<int> products, List<int> categories, List<int> subCategories, List<int> pages, Menu menu)
        {
            if (id != menu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menu);

                    _context.MenuProducts.RemoveRange(await _context.MenuProducts.Where(e => e.MenuId == menu.Id).ToListAsync());

                    products?.ForEach(p =>
                    {
                        _context.Add(new MenuProduct()
                        {
                            ProductId = p,
                            MenuId = menu.Id
                        });
                    });

                    _context.MenuCategories.RemoveRange(await _context.MenuCategories.Where(e => e.MenuId == menu.Id).ToListAsync());

                    categories?.ForEach(c =>
                    {
                        _context.Add(new MenuCategory()
                        {
                            CategoryId = c,
                            MenuId = menu.Id
                        });
                    });

                    _context.MenuSubCategories.RemoveRange(await _context.MenuSubCategories.Where(e => e.MenuId == menu.Id).ToListAsync());

                    subCategories?.ForEach(c =>
                    {
                        _context.Add(new MenuSubCategory()
                        {
                            SubCategoryId = c,
                            MenuId = menu.Id
                        });
                    });

                    _context.MenuPages.RemoveRange(await _context.MenuPages.Where(e => e.MenuId == menu.Id).ToListAsync());

                    pages?.ForEach(c =>
                    {
                        _context.Add(new MenuPage()
                        {
                            PageId = c,
                            MenuId = menu.Id
                        });
                    });


                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(menu.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
                //_context.MenuProducts.Where(a => a.MenuId == id).Select(a => a.ProductId).ToList()
            }

            ViewData["Categories"] = new MultiSelectList(_context.Categories, "Id", "Name",
                _context.MenuCategories.Where(a => a.MenuId == id).Select(e => e.CategoryId).ToArray());
            
            ViewData["SubCategories"] = new MultiSelectList(_context.SubCategories, "Id", "Name",
                _context.MenuSubCategories.Where(a => a.MenuId == id).Select(e => e.SubCategoryId).ToArray());

            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name",
                _context.MenuProducts.Where(a => a.MenuId == id).Select(e => e.ProductId).ToArray());

            ViewData["Pages"] = new MultiSelectList(_context.Pages, "Id", "Name",
                _context.MenuPages.Where(a => a.MenuId == id).Select(e => e.PageId).ToArray());


            return View(menu);
        }

        // GET: Admin/Menus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // POST: Admin/Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.Menus
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.Menus.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        public async Task<IActionResult> ResetMenu()
        {
            var menus = await _context.Menus.ToListAsync();

            _context.RemoveRange(menus);

            await _context.SaveChangesAsync();

            await _context.AddRangeAsync(new List<Menu>
            {
                new Menu
                {
                    Name = "Home",
                    Url = "/",
                    Type = "interLink",
                    ShowInFooter = true,
                    ShowInHeader = true,
                    Order = 1
                },
                new Menu
                {
                    Name = "About",
                    Url = "/about",
                    Type = "interLink",
                    ShowInFooter = true,
                    ShowInHeader = true,
                    Order = 2
                },
                new Menu
                {
                    Name = "Faculty",
                    Url = "/teachers",
                    Type = "interLink",
                    ShowInFooter = true,
                    ShowInHeader = true,
                    Order = 3
                },
                new Menu
                {
                    Name = "Courses",
                    Url = "/courses",
                    Type = "interLink",
                    ShowInFooter = true,
                    ShowInHeader = true,
                    Order = 4
                },
                new Menu
                {
                    Name = "Results",
                    Url = "/results",
                    Type = "interLink",
                    ShowInFooter = true,
                    ShowInHeader = true,
                    Order = 5
                },
                new Menu
                {
                    Name = "FAQ's",
                    Url = "/faqs",
                    Type = "interLink",
                    ShowInFooter = true,
                    ShowInHeader = true,
                    Order = 6
                },
                new Menu
                {
                    Name = "Placement",
                    Url = "/placement",
                    Type = "interLink",
                    ShowInFooter = true,
                    ShowInHeader = true,
                    Order = 7
                },
                new Menu
                {
                    Name = "Testimonials",
                    Url = "/testimonials",
                    Type = "interLink",
                    ShowInFooter = true,
                    ShowInHeader = true,
                    Order = 8
                },
                new Menu
                {
                    Name = "Updates",
                    Url = "/updates",
                    Type = "interLink",
                    ShowInFooter = true,
                    ShowInHeader = true,
                    Order = 9
                },
                new Menu
                {
                    Name = "Latest News",
                    Url = "/latestnews",
                    Type = "interLink",
                    ShowInFooter = true,
                    ShowInHeader = true,
                    Order = 10
                },
                new Menu
                {
                    Name = "Blog",
                    Url = "/blog",
                    Type = "interLink",
                    ShowInFooter = true,
                    ShowInHeader = true,
                    Order = 11
                },
                new Menu
                {
                    Name = "Gallery",
                    Url = "/gallery",
                    Type = "interLink",
                    ShowInFooter = true,
                    ShowInHeader = true,
                    Order = 12
                },
                new Menu
                {
                    Name = "Videos",
                    Url = "/videos",
                    Type = "interLink",
                    ShowInFooter = true,
                    ShowInHeader = true,
                    Order = 13
                },
                new Menu
                {
                    Name = "Franchise",
                    Url = "/franchise",
                    Type = "interLink",
                    ShowInFooter = true,
                    ShowInHeader = true,
                    Order = 14
                },
                new Menu
                {
                    Name = "Branches",
                    Url = "/branches",
                    Type = "interLink",
                    ShowInFooter = true,
                    ShowInHeader = true,
                    Order = 15
                },
                new Menu
                {
                    Name = "Contact",
                    Url = "/home/contact",
                    Type = "interLink",
                    ShowInFooter = true,
                    ShowInHeader = true,
                    Order = 16
                },
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool MenuExists(int id)
        {
            return _context.Menus.Any(e => e.Id == id);
        }
    }
}
