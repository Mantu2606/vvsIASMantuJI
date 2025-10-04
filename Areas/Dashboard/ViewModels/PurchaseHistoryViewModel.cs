using System.Collections.Generic;
using FossTech.Models.StudentModels;

namespace FossTech.Areas.Dashboard.ViewModels
{
    public class PurchaseHistoryViewModel
    {
        public IEnumerable<UserStudyMaterialAccess> StudyMaterialAccesses { get; set; }
        public IEnumerable<UserFlashCardAccess> FlashCardAccesses { get; set; }
    }
} 