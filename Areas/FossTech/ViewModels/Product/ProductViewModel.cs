using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FossTech.Models;
using FossTech.Models.ProductModels;

namespace FossTech.Areas.FossTech.ViewModels.Product
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string Slug { get; set; }

        [DisplayName("Main Image")]
        public string Image { get; set; }

        public ICollection<ProductImage> Images { get; set; }

        public string Description { get; set; }


        [DisplayName("Short Description")]
        [MaxLength(200)]
        public string ShortDescription { get; set; }


        [DisplayName("Categories")]
        public List<int> Categories { get; set; }

        [DisplayName("SubCategories")]
        public List<int> SubCategories { get; set; }

        [DisplayName("Base Price")]
        public double? BasePrice { get; set; }


        [DisplayName("Discount Amount")]
        public double? DiscountAmount { get; set; }


        [DisplayName("Product Tags")]
        public string ProductTags { get; set; }

        [DisplayName("Free Shipping")]
        public bool FreeShipping { get; set; }

        public bool Status { get; set; }

        public List<Section> Sections { get; set; }

        public ICollection<Review> Reviews { get; set; }

        [DisplayName("Final Price")]
        public double? FinalPrice
        {
            get
            {
                if (DiscountAmount == null)
                {
                    if (BasePrice != null)
                    {
                        return Math.Round((double)BasePrice);
                    }

                    return null;
                }

                if (BasePrice != null)
                    return Math.Round((double)(BasePrice - DiscountAmount), 2);
                return BasePrice;
            }
        }
        [DisplayName("Discount Percentage")]
        public double? DiscountPercentage
        {
            get
            {
                if (DiscountAmount != null) return Math.Round((double)((DiscountAmount / BasePrice) * 100), 2);
                return 0;
            }
        }
        public double? RoundedBasePrice { 
            get {
                if (BasePrice != null)
                {
                    return Math.Round((double)BasePrice, 2);
                }
                else return BasePrice;
            } 
        }

        public bool HasOptions { get;  set; }
    }
}
