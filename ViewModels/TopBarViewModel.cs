using System;
using System.Collections.Generic;
using FossTech.Models;
using System.Linq;
using System.Threading.Tasks;

namespace FossTech.ViewModels
{
    public class TopBarViewModel
    {
        public BusinessProfile BusinessProfile { get; set; }

        public List<Category> Categories { get; set; }


    }
}
