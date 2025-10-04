using System.Collections.Generic;
using FossTech.Models;

namespace FossTech.Areas.Dashboard.ViewModels
{
    public class UserHeaderViewModel
    {
        public Logo Logo { get; set; }

      public BusinessProfile BusinessProfile { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

      public int NotificationsCount { get; set; }

        public List<Notification> Notifications { get; set; }
        public string UserName { get; set; }
        public string ProfilePhoto { get; set; }

    }
}
