using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FossTech.Models
{
    public class Visitor : ITimeStampedModel
    {
        public int Id { get; set; }
        public string ClientIp { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string FullURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}
