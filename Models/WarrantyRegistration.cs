using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FossTech.Models
{
    public class WarrantyRegistration
    {
        public int Id { get; set; }

        [DisplayName("Product Name")]
        [Required]
        public string ProductName { get; set; }

        [Required]
        [DisplayName("Serial Number")]
        public string SerialNumber { get; set; }

        [DisplayName("Purchase Date")]
        [Required]
        public string PurchaseDate { get; set; }

        public string Salutation { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required]
        [DisplayName("E-mail Address")]
        public string EmailAddress { get; set; }

        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public int PostCode { get; set; }
        [Required]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
        [Required]
        [DisplayName("Mobile Number")]
        public string MobileNumber { get; set; }

        [Required]
        public bool Email { get; set; }
        public bool Mail { get; set; }
        public bool Phone { get; set; }

        [DisplayName("Invoice Copy Photo")]
        public string InvoiceCopyPhoto { get; set; }
    }
}
