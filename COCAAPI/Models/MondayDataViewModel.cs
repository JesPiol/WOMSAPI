using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace COCAAPI.Models
{
    public class MondayDataViewModel
    {
        // for item
        public string CocaNo { get; set; }
        public string PaymentType { get; set; }
        public string POBillingTerms { get; set; }
        public string ActivityDescriptionDetails { get; set; }
        public string PlannedStartDate { get; set; }
        public string PlannedEndDate { get; set; }
        public string PlannedDaysDuration { get; set; }
        public string PlannedPercentageAccomplishment { get; set; }
        public string ActualStartDate { get; set; }
        public string ActualEndDate { get; set; }
        public string ActualDaysDuration { get; set; }
        public string ActualPercentageAccomplishment { get; set; }
        public string ProjectDelayed { get; set; }
        public string ProjectExtended { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string Contractor { get; set; }
        public string ContractorPIC { get; set; }
        public string PIC { get; set; }
        public string Variance { get; set; }
        public string ObservationRemarksStatus { get; set; }
        public string JobCardNo { get; set; }
        public string LastUpdated { get; set; }
        public string WorkGroup { get; set; }
        public string SectionHead { get; set; }
        public string ProgressPicture { get; set; }
        public string ITPDocs { get; set; }

        // for subitem
        public string Sub_PONo { get; set; }
        public string Sub_ItemId { get; set; }
        public string Sub_ItemName { get; set; }
        public string Sub_DateSubmit { get; set; }
        public string Sub_Id { get; set; }
        public string Sub_IdName { get; set; }
        public string Sub_MilestoneGroup { get; set; }
        public string Sub_MilestoneName { get; set; }
        public string Sub_TaskNo { get; set; }
        public string Sub_PlannedDaysDuration { get; set; }
        public string Sub_WeightedPercentage { get; set; }
        public string Sub_PlannedStartDate { get; set; }
        public string Sub_PlannedEndDate { get; set; }
        public string Sub_TargetCompletionPercentage { get; set; }
        public string Sub_TargetRelativeWeight { get; set; }
        public string Sub_ActualStartDate { get; set; }
        public string Sub_ActualEndDate { get; set; }
        public string Sub_ActualCompletionPercentage { get; set; }
        public string Sub_ActualRelativeWeight { get; set; }
        public string Sub_PlannedStartValidation { get; set; }
        public string Sub_ActualEndValidation { get; set; }
        public string Sub_DPRDate { get; set; }
        public string Sub_MainStatus { get; set; }
        public string Sub_InspectionRequirement { get; set; }
        public string Sub_ITPStatus { get; set; }
        public string Sub_ITPRemarks { get; set; }
        public string Sub_ITPReference { get; set; }
        public string Sub_ITPUploadSigned { get; set; }
        public string Sub_MilestoneItem { get; set; }
        public string Sub_MilestoneDetails { get; set; }
        public string Sub_PaymentTermsCode { get; set; }
        public string Sub_LastUpdated { get; set; }

    }
}