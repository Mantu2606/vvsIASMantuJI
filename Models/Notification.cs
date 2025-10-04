using System;

namespace FossTech.Models
{
    public class Notification : ITimeStampedModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool Status { get; set; }

        public string Url { get; set; }  

        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}