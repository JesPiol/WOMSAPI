using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace COCAAPI.Models
{
    public class PRDetail
    {
        public string PRNo { get; set; }
        public string CocaNo { get; set; }
        public string PRDate { get; set; }
        public string PRReleasedDate { get; set; }
        public string Department { get; set; }
        public string IntendedFor { get; set; }
        public string PlantNo { get; set; }
        public string WorkDescription { get; set; }
        public string PurchaserCode { get; set; }
        public string PurchaserName { get; set; }
        public string PRMonitoringStatus { get; set; }
        public string PRApprovingStatus { get; set; }
        public string PRStatus { get; set; }
        public string PRType { get; set; }
        public string OutageCode { get; set; }
        public string OutageCodeDescription { get; set; }
        public string Priority { get; set; }
        public string PlantRelated { get; set; }
        public string TechnicalReportRequired { get; set; }
        public string ProjectLocation { get; set; }
        public string ProjectInCharge { get; set; }
        public string Company { get; set; }
        public string BudgetTotalAmount { get; set; }

        // -------------- added JBC
        public string System { get; set; }
        public string WithNTP { get; set; }
        public string NTPNumber { get; set; }
        public string WithVariationOrder { get; set; }
        // -------------- added JBC
    }

    public class PRLineViewModel
    {
        public string PRNo { get; set; }
        public string No { get; set; }
    }
}