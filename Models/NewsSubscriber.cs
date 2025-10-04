using Fingers10.ExcelExport.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace FossTech.Models
{
    public class NewsSubscriber : ITimeStampedModel
    {
        public int Id { get; set; }
        [IncludeInReport(Order = 1)]
        public string Name {  get; set; }

        [Required]
        [IncludeInReport(Order = 2)]
        public string Email { get; set; }
        [IncludeInReport(Order = 3)]
        public bool Status { get; set; }
        [IncludeInReport(Order = 4)]
        public DateTime Created { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}