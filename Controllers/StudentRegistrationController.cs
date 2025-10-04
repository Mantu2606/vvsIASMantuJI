using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Areas.FossTech.ViewModels;
using FossTech.Data;
using FossTech.Extensions;
using FossTech.Models;
using System;
using System.IO;
using FossTech.Models.StudentModels;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace FossTech.Areas.FossTech.Controllers
{
    public class StudentRegistrationController : Controller
    {
      
            private readonly ApplicationDbContext _context;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;


            public StudentRegistrationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
            {
                _context = context;
                _userManager = userManager;
                _roleManager = roleManager;
            }

            // GET: Users
            public IActionResult Index()
            {
                ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
                ViewData["Products"] = new SelectList(_context.Products, "Id", "Name");
                return View();
            }

            // POST: Users/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Index(StudentProfileViewModel Input)
            {
                if (ModelState.IsValid)
                {

                    var user = new ApplicationUser
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        PhoneNumber = Input.MobileNumber,
                        DateOfBirth = Input.DateOfBirth,
                        Gender = Input.Gender,
                        isStudent = true,
                        BranchId = Input.BranchId,  
                        CreatedAt = DateTime.Now,
                        LastModified = DateTime.Now,
                    };

                    if (Input.ProfilePhoto != null)
                    {

                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/ProfileImage")))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                                "wwwroot/img/ProfileImage"));
                        }
                        var mainImageName = Guid.NewGuid() + Path.GetExtension(Input.ProfilePhoto.FileName);

                        //Get url To Save
                        var mainImageSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/ProfileImage", mainImageName);

                        await using (var stream = new FileStream(mainImageSavePath, FileMode.Create))
                        {
                            await Input.ProfilePhoto.CopyToAsync(stream);
                        }

                        user.ProfilePhoto = $"/img/ProfileImage/{mainImageName}";
                    }

                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        
                        await _userManager.AddToRolesAsync(user, new [] { "User" });

                        await _context.SaveChangesAsync();

                        foreach (var studentCourses in Input.Courses)
                        {
                            var addStudentCourses = new StudentCourse
                            {
                                UserId = user.Id,
                                ProductId = studentCourses,
                            };

                            _context.Add(addStudentCourses);
                        }

                        await _context.SaveChangesAsync();  

                        return RedirectToAction(nameof(Index), "Home").WithSuccess("Your Account has been created Successfully.", null);
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new SelectList(_context.Products, "Id", "Name");

            return View(Input).WithError("Please fill the correct details.", null);
            }

           
        }
    }