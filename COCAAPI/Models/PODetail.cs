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
        public int VersionNo { get; set; }
    }

    public class POLineViewModel
    {
        public string PRNo { get; set; }
        public string No { get; set; }
    }
}