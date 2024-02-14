using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace COCAAPI.Models
{
    public class MondayData
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string PIC { get; set; }
        public string Contractors { get; set; }
        public string PlannedStartDate { get; set; }
        public string PlannedEndDate { get; set; }
        public string PlannedDaysDuration { get; set; }
        public string TargetCompletionPercentage { get; set; }
        public string ActualStartSate { get; set; }
        public string ActualEndDate { get; set; }
        public string ActualDaysDuration { get; set; }
        public string ActualCompletionPercentage { get; set; }
        public string Variance { get; set; }
        public string ObservationRemarksStatus { get; set; }
        public string LastUpdated { get; set; }
    }
}