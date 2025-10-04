using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FossTech.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FossTech.ViewComponents
{
    [ViewComponent(Name = "FooterCode")]
    public class FooterCode : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public FooterCode(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var footerCode = new Models.FooterCode();
            if (_context.FooterCodes.Any())
            {
                footerCode = await _context.FooterCodes.FirstOrDefaultAsync();
            }

            return View("Index", footerCode);
        }
    }
}
