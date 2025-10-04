using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FossTech.Data;
using FossTech.Models;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;

namespace FossTech.Services
{

    public interface INotificationService
    {
        public Task<int> GetNotificationsCount();
        public Task<List<Notification>> GetNotifications();
        public Task<Notification> AddNotification(Notification model);
        public Task<List<Notification>> MarkAllAsRead();
    }

    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetNotificationsCount()
        {
            var items = await _context.Notifications
                .Where(x => x.Status == false)
                .CountAsync();

            return items;
        }

        public async Task<List<Notification>> GetNotifications()
        {
            var items = await _context.Notifications
                .Where(x => x.Status == false)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return items;
        }

        public async Task<Notification> AddNotification(Notification model)
        {
            var notification = new Notification
            {
                Status = false,
                Title = model.Title,
                Description = model.Description,
                Url = model.Url,
                CreatedAt = DateTime.Now
            };

            _context.Add(notification);
            await _context.SaveChangesAsync();

            return notification;
        }

        public async Task<List<Notification>> MarkAllAsRead()
        {
            var notifications = await _context.Notifications.Where(x => x.Status == false).ToListAsync();

            foreach (var notification in notifications)
            {
                notification.Status = true;
                notification.LastModified = DateTime.Now;
                _context.Update(notification);
            }

            await _context.SaveChangesAsync();

            return notifications;
        }
    }
}