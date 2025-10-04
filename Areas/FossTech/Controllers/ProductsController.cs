using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FossTech.Data;
using FossTech.Extensions;
using FossTech.Models;
using FossTech.Models.ProductModels;
using X.PagedList;
using Fingers10.ExcelExport.ActionResults;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index(string category, string subCategory, string query, int minPrice, int maxPrice, int pageindex = 1, int PageSize = 600, string orderby = null)
        {


            var products = await _context.Products
                .Include(x => x.Section)
                .AsNoTracking()
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> ExporttoExcel()
        {
            var results = await _context.Products.ToListAsync();
            return new ExcelResult<Product>(results, "Demo Sheet Name", "Fingers10");
        }


        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                //.Include(p => p.Category)
                //.Include(p => p.SubCategory)
                .Include(i => i.Images)
                .Include(c => c.ProductCategories)
                .ThenInclude(c => c.Category)
                .Include(sc => sc.ProductSubCategories)
                .ThenInclude(sc => sc.SubCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["Categories"] = new MultiSelectList(_context.Categories, "Id", "Name");
            ViewData["SubCategories"] = new MultiSelectList(_context.SubCategories, "Id", "Name");
            ViewData["SectionId"] = new MultiSelectList(_context.Sections, "Id", "Name");

            return View();
        }
        public IActionResult Create1()
        {
            ViewData["Categories"] = new MultiSelectList(_context.Categories, "Id", "Name");
            ViewData["SubCategories"] = new MultiSelectList(_context.SubCategories, "Id", "Name");
            ViewData["SectionId"] = new MultiSelectList(_context.Sections, "Id", "Name");


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile mainImage, IFormFile productPDF, List<IFormFile> images, Product product, List<int> categories, List<int> subCategories, List<int> sections)
        {
            if (ModelState.IsValid)
            {
                if (productPDF != null)
                {
                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products/PDFs")))
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products/PDFs"));
                    }
                    var myPdf = Guid.NewGuid() + Path.GetExtension(productPDF.FileName);

                    // Get URL to save
                    var mypdfSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products/PDFs", myPdf);

                    await using (var stream = new FileStream(mypdfSavePath, FileMode.Create))
                    {
                        await productPDF.CopyToAsync(stream);
                    }

                    product.PDF = $"/img/products/PDFs/{myPdf}";
                }

                if (mainImage == null)
                {
                    ModelState.AddModelError("Image", "Product Image is required.");
                    return View(product);
                }
                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products"));
                }
                var mainImageName = Guid.NewGuid() + Path.GetExtension(mainImage.FileName);

                // Get URL to save
                var mainImageSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products", mainImageName);

                await using (var stream = new FileStream(mainImageSavePath, FileMode.Create))
                {
                    await mainImage.CopyToAsync(stream);
                }

                product.Image = $"/img/products/{mainImageName}";
                _context.Add(product);
                await _context.SaveChangesAsync();

                if (categories != null)
                {
                    categories.ForEach(c =>
                    {
                        _context.Add(new ProductCategory()
                        {
                            CategoryId = c,
                            ProductId = product.Id
                        });
                    });
                    await _context.SaveChangesAsync();
                }

                if (subCategories != null)
                {
                    subCategories.ForEach(c =>
                    {
                        _context.Add(new ProductSubCategory()
                        {
                            SubCategoryId = c,
                            ProductId = product.Id
                        });
                    });
                    await _context.SaveChangesAsync();
                }



                if (images != null)
                {
                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products")))
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products"));
                    }

                    foreach (IFormFile item in images)
                    {
                        // Set key name
                        var imageName = Guid.NewGuid() + Path.GetExtension(item.FileName);

                        // Get URL to save
                        var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products", imageName);

                        await using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await item.CopyToAsync(stream);
                        }

                        var image = new ProductImage()
                        {
                            ProductId = product.Id,
                            Img = $"/img/products/{imageName}"
                        };

                        _context.Add(image);
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["Categories"] = new MultiSelectList(_context.Categories, "Id", "Name");
            ViewData["SubCategories"] = new MultiSelectList(_context.SubCategories, "Id", "Name");
            ViewData["SectionId"] = new MultiSelectList(_context.Sections, "Id", "Name");

            return View(product);
        }

        public async Task<IActionResult> Edit1(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(i => i.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["SectionId"] = new MultiSelectList(_context.Sections, "Id", "Name");

            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(i => i.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["Categories"] = new MultiSelectList(_context.Categories, "Id", "Name",
               _context.ProductCategories.Where(a => a.ProductId == id).Select(e => e.CategoryId).ToArray());

            ViewData["SubCategories"] = new MultiSelectList(_context.SubCategories, "Id", "Name",
               _context.ProductSubCategories.Where(a => a.ProductId == id).Select(e => e.SubCategoryId).ToArray());

            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name", product.SectionId);
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile mainImage, List<IFormFile> images, IFormFile productPDF, List<int> Taxes, List<int> categories, List<int> subCategories, List<int> productVariants, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var thisProduct = await _context.Products.FirstOrDefaultAsync(i => i.Id == product.Id);
                    if (thisProduct == null)
                    {
                        return NotFound();
                    }

                    if (productPDF != null)
                    {
                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products/PDFs")))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                                "wwwroot/img/products/PDFs"));
                        }
                        var myPdf = Guid.NewGuid() + Path.GetExtension(productPDF.FileName);

                        //Get url To Save
                        var mypdfSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products/PDFs", myPdf);

                        await using (var stream = new FileStream(mypdfSavePath, FileMode.Create))
                        {
                            await productPDF.CopyToAsync(stream);
                        }

                        thisProduct.PDF = $"/img/products/PDFs/{myPdf}";
                    }

                    if (mainImage != null)
                    {
                        var mainImageName = Guid.NewGuid() + Path.GetExtension(mainImage.FileName);

                        //Get url To Save
                        var mainImageSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products", mainImageName);

                        await using (var stream = new FileStream(mainImageSavePath, FileMode.Create))
                        {
                            await mainImage.CopyToAsync(stream);
                        }

                        thisProduct.Image = $"/img/products/{mainImageName}";
                    }

                    if (images != null)
                    {
                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products")))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                                "wwwroot/img/products"));
                        }

                        foreach (IFormFile item in images)
                        {
                            //Set Key Name
                            var imageName = Guid.NewGuid() + Path.GetExtension(item.FileName);

                            //Get url To Save
                            var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products", imageName);

                            await using (var stream = new FileStream(savePath, FileMode.Create))
                            {
                                await item.CopyToAsync(stream);
                            }

                            var image = new ProductImage()
                            {
                                ProductId = product.Id,
                                Img = $"/img/products/{imageName}"
                            };

                            _context.Add(image);
                        }
                    }

                    thisProduct.Name = product.Name;
                    thisProduct.EnableTaxes = product.EnableTaxes;
                    thisProduct.Description = product.Description;
                    //thisProduct.SubCategoryId = product.SubCategoryId;
                    thisProduct.BasePrice = product.BasePrice;
                    thisProduct.DiscountAmount = product.DiscountAmount;
                    thisProduct.FreeShipping = product.FreeShipping;
                    thisProduct.Status = product.Status;
                    thisProduct.ProductTags = product.ProductTags;
                    thisProduct.ShortDescription = product.ShortDescription;
                    thisProduct.Slug = product.Slug;
                    thisProduct.ShowButtons = product.ShowButtons;
                    thisProduct.InStock = product.InStock;
                    thisProduct.OutOfStock = product.OutOfStock;
                    thisProduct.Schema = product.Schema;
                    thisProduct.SortOrder = product.SortOrder;
                    thisProduct.MetaTitle = product.MetaTitle;
                    thisProduct.MetaDescription = product.MetaDescription;
                    thisProduct.MetaKeywords = product.MetaKeywords;
                    thisProduct.ImgAlt = product.ImgAlt;
                    thisProduct.SectionId = product.SectionId;

                    _context.ProductCategories.RemoveRange(await _context.ProductCategories.Where(e => e.ProductId == product.Id).ToListAsync());

                    categories?.ForEach(c =>
                    {
                        _context.Add(new ProductCategory()
                        {
                            CategoryId = c,
                            ProductId = product.Id
                        });
                    });
                    _context.ProductSubCategories.RemoveRange(await _context.ProductSubCategories.Where(e => e.ProductId == product.Id).ToListAsync());

                    subCategories?.ForEach(c =>
                    {
                        _context.Add(new ProductSubCategory()
                        {
                            SubCategoryId = c,
                            ProductId = product.Id
                        });
                    });

                    _context.Update(thisProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
           

                return RedirectToAction(nameof(Index));
            }

            ViewData["Categories"] = new MultiSelectList(_context.Categories, "Id", "Name",
              _context.ProductCategories.Where(a => a.ProductId == id).Select(e => e.CategoryId).ToArray());

            ViewData["SubCategories"] = new MultiSelectList(_context.SubCategories, "Id", "Name",
               _context.ProductSubCategories.Where(a => a.ProductId == id).Select(e => e.SubCategoryId).ToArray());

            ViewData["SectionId"] = new MultiSelectList(_context.Sections, "Id", "Name");

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> ImageUpload(IFormFile file)
        {
            if (file != null)
            {
                //Set Key Name
                var imageName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/uploads")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/uploads"));
                }

                //Get url To Save
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/uploads", imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return Content($"/img/uploads/{imageName}");
            }
            return BadRequest();
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                //.Include(p => p.Category)
                //.Include(p => p.SubCategory)
                .Include(i => i.Images)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> leadIds)
        {

            var leadsToDelete = await _context.Products
            .Where(lead => leadIds.Contains(lead.Id))
            .ToListAsync();

            _context.Products.RemoveRange(leadsToDelete);
            await _context.SaveChangesAsync();


            return Ok("Success");
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }


        public async Task<IActionResult> DeleteProductImage(int imageId, int productId)
        {
            var productImage = await _context.ProductImages.FindAsync(imageId);
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = productId });
        }
    }
}
