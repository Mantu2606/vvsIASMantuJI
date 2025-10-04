using System;

namespace FossTech.Models
{
    public class LoginPageImage : ITimeStampedModel
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}