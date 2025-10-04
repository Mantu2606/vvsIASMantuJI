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

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    public class WebSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WebSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FossTech/WebSettings
        public async Task<IActionResult> Index()
        {
            var webSettings = await _context.WebSettings.OrderBy(x => x.Id).FirstOrDefaultAsync();
            if (webSettings == null)
            {
                webSettings = new WebSetting();
            }
            return View(webSettings);
        }




        // POST: FossTech/WebSettings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(WebSetting webSetting)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    if (webSetting.Id != 0)
                    {
                        _context.Update(webSetting);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Add(webSetting);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WebSettingExists(webSetting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                string referringUrl = Request.Headers["Referer"].ToString();
                return Redirect(referringUrl).WithSuccess("Web Settings has been Updated.", null);
            }
            return View(webSetting);
        }


        private bool WebSettingExists(int id)
        {
            return (_context.WebSettings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}