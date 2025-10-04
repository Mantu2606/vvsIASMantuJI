using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FossTech.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FossTech.ViewComponents
{
    [ViewComponent(Name = "HeaderCode")]
    public class HeaderCode : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public HeaderCode(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var headerCode = new Models.HeaderCode();
            if (_context.HeaderCodes.Any())
            {
                headerCode = await _context.HeaderCodes.FirstOrDefaultAsync();
            }

            return View("Index", headerCode);
        }
    }
}
