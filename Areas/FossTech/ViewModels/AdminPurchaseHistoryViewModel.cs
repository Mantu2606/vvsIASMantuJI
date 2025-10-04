using System.Collections.Generic;
using FossTech.Models.StudentModels;
using FossTech.Models.StudyMaterialModels;

namespace FossTech.Areas.FossTech.ViewModels
{
    public class AdminPurchaseHistoryViewModel
    {
        public IEnumerable<UserStudyMaterialAccess> StudyMaterialAccesses { get; set; }
        public IEnumerable<UserFlashCardAccess> FlashCardAccesses { get; set; }
        public IEnumerable<UserStudyMaterialAccess> PendingStudyMaterials { get; set; }
        public IEnumerable<UserFlashCardAccess> PendingFlashCards { get; set; }

        // Filter dropdown lists
        public IEnumerable<Board> Boards { get; set; }
        public IEnumerable<Standard> Standards { get; set; }
        public IEnumerable<Subject> Subjects { get; set; }
        public IEnumerable<string> PaymentStatuses { get; set; }

        // Selected filter values
        public int? SelectedBoardId { get; set; }
        public int? SelectedStandardId { get; set; }
        public int? SelectedSubjectId { get; set; }
        public string SelectedPaymentStatus { get; set; }
    }
} 