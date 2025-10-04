using FossTech.Models.ProductModels;
using System;

namespace FossTech.Models
{
    public class WishList : ITimeStampedModel
    {
        public int Id { get; set; }

        public ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public Product Product { get; set; }
        public int ProductId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}