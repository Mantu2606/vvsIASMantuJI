using System;

namespace FossTech.Models
{
    public class CallClickCount : ITimeStampedModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}
