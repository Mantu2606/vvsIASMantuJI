using System.Collections.Generic;
using FossTech.Models;

namespace FossTech.Areas.FossTech.ViewModels
{
    public class HeaderViewModel
    {
        public Logo Logo { get; set; }

      public BusinessProfile BusinessProfile { get; set; }

      public int NotificationsCount { get; set; }

      public List<Notification> Notifications { get; set; }
        public bool IsUnderConstruction { get; set; }
        public int Id { get; set; }
    }
}
