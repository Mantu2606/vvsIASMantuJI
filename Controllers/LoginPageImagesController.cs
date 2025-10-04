using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using FossTech.Data;
using FossTech.Models;

namespace FossTech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginPageImagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoginPageImagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/LoginPageImages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoginPageImage>>> GetLoginPageImages()
        {
            return await _context.LoginPageImages.ToListAsync();
        }
    }
}
