using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace COCAAPI.Models
{
    public class PODetail
    {
        public string PRNo { get; set; }
        public string PONo { get; set; }
        public string DateArchived { get; set; }
        public string BuyFromVendorNo { get; set; }
        public string BuyFromVendorName { get; set; }
        public string POStatus { get; set; }
        public string POTotalLineAmount { get; set; }
        public string PaymentType { get; set; }
        public string POBillingTerms { get; set; }
        public int NoOfProgressBilling { get; set; }
        public string PBMilestone { get; set; }
        public string Company { get; set; }
        public string PlantNo { get; set; }
        public int VersionNo { get; set; }

        // -------------- added JBC

        public string OrderDate { get; set; }
        public string POPaymentTerms { get; set; }
        public string POContractAmount { get; set; }
        public string POAmendedAmount { get; set; }
        public string RetentionPercent { get; set; }
        public string RetentionAmount { get; set; }
        public string RetentionReleaseDate { get; set; }
        public string ProgressBillingPercent { get; set; }
        // -------------- added JBC
    }

    public class POLineViewModel
    {
        public string PRNo { get; set; }
        public string No { get; set; }
        public string DocumentNo { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string WarrantyDurationStartDate { get; set; }
        public string WarrantyDurationEndDate { get; set; }
    }
}