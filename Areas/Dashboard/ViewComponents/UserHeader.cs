using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FossTech.Areas.Dashboard.ViewModels;
using FossTech.Data;
using FossTech.Services;
using Microsoft.AspNetCore.Identity;
using FossTech.Models;

namespace FossTech.Areas.Dashboard.ViewComponents
{
    [ViewComponent(Name = "UserHeader")]
    public class UserHeader : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserHeader(ApplicationDbContext context, INotificationService notificationService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            
            var logo = await _context.Logos.OrderBy(x => x.Id).FirstOrDefaultAsync();
            var bussinessProfile = await _context.BusinessProfile.OrderBy(x => x.Id).FirstOrDefaultAsync();
            ApplicationUser user= await _userManager.GetUserAsync(HttpContext.User);

            var model = new UserHeaderViewModel
            {
                Logo = logo,
                BusinessProfile = bussinessProfile,
                Notifications = await _notificationService.GetNotifications(),
                NotificationsCount = await _notificationService.GetNotificationsCount(),
                UserName = user.FullName,
                ProfilePhoto=user.ProfilePhoto,
               
               
            }; 

            return View("Index", model);
        }
    }
}