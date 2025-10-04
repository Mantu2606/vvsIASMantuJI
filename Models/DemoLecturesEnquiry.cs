using System;
using System.Collections.Generic;

namespace FossTech.Models
{
    public class DemoLecturesEnquiry : ITimeStampedModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int? BranchId { get; set; }
        public Branch Branch { get; set; }
        public bool IsFoundation { get; set; }
        public virtual List<DemoLectureCoursesList> Courses { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}
