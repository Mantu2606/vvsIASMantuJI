using Fingers10.ExcelExport.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FossTech.Models.ProductModels
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [IncludeInReport(Order = 1)]
        public string Name { get; set; }
        [IncludeInReport(Order = 2)]
        public string Slug { get; set; }

        [IncludeInReport(Order = 3)]
        [DisplayName("Main Image")]
        public string Image { get; set; }

        [IncludeInReport(Order = 4)]
        [DisplayName("Banner Image")]
        public string BannerImage { get; set; }

        [IncludeInReport(Order = 5)]
        public ICollection<ProductImage> Images { get; set; }

        public string PDF { get; set; }

        [IncludeInReport(Order = 6)]
        public string Description { get; set; }

        [IncludeInReport(Order = 7)]
        [DisplayName("Short Description")]
        public string ShortDescription { get; set; }
        public string Schema { get; set; }

        public bool HasOptions { get; set; }
        public bool InStock { get; set; }
        public bool OutOfStock { get; set; }

        //[Display(Name = "Category")]
        //public int? CategoryId { get; set; }

        //[ForeignKey("CategoryId")]
        //public virtual Category Category { get; set; }

        //[Display(Name = "Sub Category")]
        //public int? SubCategoryId { get; set; }

        //[DisplayName("Sub Category")]
        //[ForeignKey("SubCategoryId")]
        //public virtual SubCategory SubCategory { get; set; }

        [IncludeInReport(Order = 8)]
        [DisplayName("Product Categories")]
        public ICollection<ProductCategory> ProductCategories { get; set; }


        [IncludeInReport(Order = 9)]
        [DisplayName("Product SubCategories")]
        public ICollection<ProductSubCategory> ProductSubCategories { get; set; }
        public ICollection<ProdutFaq> ProdutFaq { get; set; }

        [IncludeInReport(Order = 10)]
        [DisplayName("Base Price")]
        public double? BasePrice { get; set; }

        [IncludeInReport(Order = 11)]
        [DisplayName("Discount Amount")]
        public double? DiscountAmount { get; set; }

        [IncludeInReport(Order = 12)]
        [DisplayName("Product Tags")]
        public string ProductTags { get; set; }

        [IncludeInReport(Order = 13)]
        [DisplayName("Free Shipping")]
        public bool FreeShipping { get; set; }

        [DisplayName("Show Buttons")]
        public bool ShowButtons { get; set; } = true;

        [IncludeInReport(Order = 14)]
        public bool Status { get; set; }

        [IncludeInReport(Order = 15)]
        [DisplayName("Enable Taxes")]
        public bool EnableTaxes { get; set; }

        [IncludeInReport(Order = 16)]
        [DisplayName("Sort Order")]
        public int SortOrder { get; set; }

        [IncludeInReport(Order = 17)]
        public DateTime CreatedAt { get; set; }

        public ICollection<Review> Reviews { get; set; }
      

        [DisplayName("Final Price")]
        public double? FinalPrice =>
            BasePrice != null
                ? DiscountAmount == null ? BasePrice : Math.Round((double)(BasePrice - DiscountAmount), 2)
                : null;

        [DisplayName("Discount Percentage")]
        public double? DiscountPercentage =>
            DiscountAmount == null ? null :
            BasePrice != null ? (double?) Math.Round((double) (DiscountAmount / BasePrice * 100), 2) : null;

        public double? RoundedBasePrice => BasePrice != null ? (double?) Math.Round((double) BasePrice, 2) : null;
        public string Brochure { get; set; }

        public string  ImgAlt { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; } 
        public string MetaKeywords { get; set; }

        public int? SectionId { get; set; }
        public Section Section { get; set; }
    }
}
