using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FossTech.Areas.FossTech.ViewModels;
using FossTech.Data;
using FossTech.Extensions;
using FossTech.Models;
using FossTech.Models.StudentModels;
using System.Globalization;
using CsvHelper;
using System.Threading.Tasks;
using FossTech.Models.StudyMaterialModels;

namespace FossTech.Areas.FossTech.Controllers
{
    [Area("FossTech")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public StudentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var users = _userManager.Users
                .Where(x => x.isStudent)
                .Include(x => x.Branch)
                .Include(x => x.Board)
                .Include(x => x.Standard)
                .Include(x => x.Courses)
                .ThenInclude(x => x.Product)
                .ToList();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkCreate(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/studentcsv");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var filePath = Path.Combine(directoryPath, Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var users = new List<ApplicationUser>();
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<StudentCsvModel>().ToList();

                foreach (var record in records)
                {
                    var branch = await _context.Branches.FirstOrDefaultAsync(b => b.BranchName == record.Branch);
                    var board = await _context.Boards.FirstOrDefaultAsync(b => b.Name == record.Board);
                    var standard = await _context.Standards.FirstOrDefaultAsync(s => s.Name == record.Standard);

                    if (branch == null || board == null || standard == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Branch, Board, or Standard provided.");
                        continue;
                    }

                    var user = new ApplicationUser
                    {
                        UserName = record.Email,
                        Email = record.Email,
                        FirstName = record.FirstName,
                        LastName = record.LastName,
                        PhoneNumber = record.MobileNumber,
                        DateOfBirth = record.DateOfBirth,
                        Gender = record.Gender,
                        isStudent = true,
                        BranchId = branch.Id,
                        BoardId = board.Id,
                        StandardId = standard.Id,
                        CreatedAt = DateTime.Now,
                        LastModified = DateTime.Now,
                    };

                    if (!string.IsNullOrEmpty(record.ProfilePhoto))
                    {
                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/ProfileImage")))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/ProfileImage"));
                        }

                        var profileImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/ProfileImage", Path.GetFileName(record.ProfilePhoto));
                        if (System.IO.File.Exists(record.ProfilePhoto))
                        {
                            System.IO.File.Copy(record.ProfilePhoto, profileImagePath);
                            user.ProfilePhoto = $"/img/ProfileImage/{Path.GetFileName(record.ProfilePhoto)}";
                        }
                    }

                    var result = await _userManager.CreateAsync(user, "DefaultPassword123");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRolesAsync(user, new[] { "User" });
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }

            return RedirectToAction(nameof(Index)).WithSuccess("Users have been added successfully.");
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.Where(x => x.isStudent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        public ActionResult Create(int? boardId, int? standardId)
        {
            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name", boardId);
            if (boardId.HasValue)
            {
                ViewData["StandardId"] = new SelectList(_context.Standards.Where(s => s.BoardId == boardId), "Id", "Name", standardId);
            }
            else
            {
                ViewData["StandardId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new SelectList(_context.Products, "Id", "Name");

            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentProfileViewModel Input)
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
                    BoardId = Input.BoardId,
                    StandardId = Input.StandardId,
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

                    await _userManager.AddToRolesAsync(user, new[] { "User" });

                    await _context.SaveChangesAsync();

                    if (Input.Courses != null)
                    {
                        foreach (var studentCourses in Input.Courses)
                        {
                            var addStudentCourses = new StudentCourse
                            {
                                UserId = user.Id,
                                ProductId = studentCourses,
                            };

                            _context.Add(addStudentCourses);
                        }
                    }

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index)).WithSuccess("User has been added.", null);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name", Input.BoardId);
            ViewData["StandardId"] = new SelectList(_context.Standards.Where(s => s.BoardId == Input.BoardId), "Id", "Name", Input.StandardId);
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName");
            ViewData["Products"] = new SelectList(_context.Products, "Id", "Name");

            return View(Input).WithError("Please fill the correct details.", null);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            UpdateStudentProfileViewModel updateUser = new UpdateStudentProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                MobileNumber = user.PhoneNumber,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                BranchId = user.BranchId,
                BoardId = user.BoardId,
                StandardId = user.StandardId,
                ProfileImage = user.ProfilePhoto
            };
            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name", user.BoardId);
            ViewData["StandardId"] = new SelectList(_context.Standards.Where(s => s.BoardId == user.BoardId), "Id", "Name", user.StandardId);
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName", user.BranchId);
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name", _context.StudentCourses.Where(x => x.UserId == user.Id).Select(x => x.ProductId).ToArray());

            return View(updateUser);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateStudentProfileViewModel user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var AUser = await _userManager.FindByIdAsync(id);
                    AUser.FirstName = user.FirstName;
                    AUser.LastName = user.LastName;
                    AUser.Email = user.Email;
                    AUser.UserName = user.Email;
                    AUser.PhoneNumber = user.MobileNumber;
                    AUser.DateOfBirth = user.DateOfBirth;
                    AUser.Gender = user.Gender;
                    AUser.BranchId = user.BranchId;
                    AUser.BoardId = user.BoardId;
                    AUser.StandardId = user.StandardId;
                    AUser.ProfilePhoto = user.ProfileImage;

                    AUser.LastModified = DateTime.Now;

                    if (user.ProfilePhoto != null)
                    {
                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/ProfileImage")))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                                "wwwroot/img/ProfileImage"));
                        }
                        var mainImageName = Guid.NewGuid() + Path.GetExtension(user.ProfilePhoto.FileName);

                        //Get url To Save
                        var mainImageSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/ProfileImage", mainImageName);

                        await using (var stream = new FileStream(mainImageSavePath, FileMode.Create))
                        {
                            await user.ProfilePhoto.CopyToAsync(stream);
                        }

                        AUser.ProfilePhoto = $"/img/ProfileImage/{mainImageName}";
                    }

                    await _userManager.UpdateAsync(AUser);

                    if (user.Password != null)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(AUser);

                        await _userManager.ResetPasswordAsync(AUser, token, user.Password);

                        _context.RemoveRange(await _context.StudentCourses.Where(x => x.UserId == AUser.Id).ToListAsync());
                        await _context.SaveChangesAsync();

                        if (user.Courses != null)
                        {
                            foreach (var studentCourses in user.Courses)
                            {
                                var addStudentCourses = new StudentCourse
                                {
                                    UserId = user.Id,
                                    ProductId = studentCourses,
                                };

                                _context.Add(addStudentCourses);
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)).WithSuccess("User has been updated.", null);
            }
            ViewData["BoardId"] = new SelectList(_context.Boards, "Id", "Name", user.BoardId);
            ViewData["StandardId"] = new SelectList(_context.Standards.Where(s => s.BoardId == user.BoardId), "Id", "Name", user.StandardId);
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "BranchName", user.BranchId);
            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name", _context.StudentCourses.Where(x => x.UserId == user.Id).Select(x => x.ProductId).ToArray());

            return View(user).WithError("Please fill the correct details.", null);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var product = await _context.Users.FindAsync(id);
            _context.Users.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMulitple(string id, IFormCollection collection)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                await _userManager.DeleteAsync(user);
                return RedirectToAction(nameof(Index)).WithSuccess("User has been removed.", null);
            }
            catch
            {
                return View().WithError("Something went wrong.", null);
            }
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
    public class StudentCsvModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Branch { get; set; }
        public string Board { get; set; }
        public string Standard { get; set; }
        public string ProfilePhoto { get; set; }  // Path or URL to profile photo
    }
}