using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FossTech.Models
{
    interface ITimeStampedModel
    {
        DateTime CreatedAt { get; set; }
        DateTime LastModified { get; set; }
    }
}
