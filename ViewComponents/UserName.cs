using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FossTech.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FossTech.ViewComponents
{
    [ViewComponent(Name = "UserName")]
    public class UserName:ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public UserName(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Id == claims.Value);
            return View(userFromDb);
        }

    }
}
