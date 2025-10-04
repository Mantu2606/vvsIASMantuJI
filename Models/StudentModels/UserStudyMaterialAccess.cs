using System;
using FossTech.Models.StudyMaterialModels;

namespace FossTech.Models.StudentModels
{
    public class UserStudyMaterialAccess
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int StudyMaterialId { get; set; }
        public StudyMaterial StudyMaterial { get; set; }
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime PurchasedAt { get; set; }
        public string PhonePayOrderId { get;  set; }
    }
} 