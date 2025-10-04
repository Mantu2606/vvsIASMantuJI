using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FossTech.Areas.FossTech.ViewModels;
using FossTech.Data;
using FossTech.Services;

namespace FossTech.Areas.FossTech.ViewComponents
{
    [ViewComponent(Name = "AdminHeader")]
    public class AdminHeader : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public AdminHeader(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            
            var logo = await _context.Logos.OrderBy(x => x.Id).FirstOrDefaultAsync();
            var bussinessProfile = await _context.BusinessProfile.OrderBy(x => x.Id).FirstOrDefaultAsync();

            var model = new HeaderViewModel
            {
                Logo = logo,
                BusinessProfile = bussinessProfile,
                Notifications = await _notificationService.GetNotifications(),
                NotificationsCount = await _notificationService.GetNotificationsCount(),
                IsUnderConstruction = await _context.WebSettings.Select(x => x.IsUnderConstruction).FirstOrDefaultAsync(),
                Id = await _context.WebSettings.Select(x => x.Id).FirstOrDefaultAsync(),
            }; 

            return View("Index", model);
        }
    }
}