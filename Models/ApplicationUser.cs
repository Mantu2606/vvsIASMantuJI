using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FossTech.Models.StudentModels;
using FossTech.Models.StudyMaterialModels;
using Microsoft.AspNetCore.Identity;

namespace FossTech.Models
{
    public class ApplicationUser : IdentityUser, ITimeStampedModel
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string ProfilePhoto { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public bool isStudent { get; set; }
        public int? BranchId { get; set; }
        public Branch Branch { get; set; }
        public int? BoardId { get; set; }

        public Board Board { get; set; }
        public int? StandardId { get; set; }

        public Standard Standard { get; set; }
        public virtual List<StudentCourse> Courses { get; set; }
        public virtual IEnumerable<Address> Addresses { get; set; }
        public virtual IEnumerable<WishList> WishList { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }

}
