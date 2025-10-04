using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FossTech.Models
{
    public class BankAccount : ITimeStampedModel
    {
        public int Id { get; set; }

        [DisplayName("Name Of The Bank")]
        public string BankName { get; set; }

        [DisplayName("Name Of The Account Holder")]
        public string AccountName { get; set; }

        [DisplayName("Bank Account Number")]
        public string AccountNumber { get; set; }

        [DisplayName("Bank IFSC Code")]
        public string BankIfsc { get; set; }

        [DisplayName("Aadhaar Number")]
        public string AadhaarNumber { get; set; }

        [DisplayName("PAN Number")]
        public string PanNumber { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}
