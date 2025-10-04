using System;
using System.ComponentModel.DataAnnotations;

namespace FossTech.Models
{
    public class Contact : ITimeStampedModel
    {
        public int Id { get; set; }
        [Required]
        public string Phone { get; set; }
        public string AlternetPhone { get; set; }

        public string Email { get; set; }
        public string StreetAddress { get; set; }
        public string city { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }

        public string Country { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }

    }
}
