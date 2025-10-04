using FossTech.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class RobotsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RobotsController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "robots.txt");
            if (System.IO.File.Exists(filepath))
            {
                var content = System.IO.File.ReadAllText(filepath);
                return View(new RobotViewModel
                {
                    Content = content
                });
            }
            return View();
        }

        [HttpPost]
        public IActionResult Index(RobotViewModel model)
        {
            var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "robots.txt");
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.WriteAllText(filepath , model.Content);
                return RedirectToAction("Index").WithSuccess("Saved");
            }
            return View();
        }
    }
    public class RobotViewModel
    {
        public string Content { get; set; }
    }
}
