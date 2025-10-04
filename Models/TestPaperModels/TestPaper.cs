

using FossTech.Models.ProductModels;
using System;
using System.Collections.Generic;

namespace FossTech.Models.TestPaperModels
{
    public class TestPaper : ITimeStampedModel
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string ThumbnailImage { get; set; }
        public virtual List<TestPaperPDF> PDFs { get; set; }
        public int? SortOrder { get; set; }  
        public int ProductId { get; set; }
        public Product Product { get; set; }    
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}
