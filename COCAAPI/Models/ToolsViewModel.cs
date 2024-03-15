using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace COCAAPI.Models
{
    public class ToolsViewModel
    {
        public string ItemCode { get; set; }
        public string ToolCode { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string QuantityIssued { get; set; }
        public string DateIssued { get; set; }
        public string PONo { get; set; }
        public string UnitCost { get; set; }
        public string DocStatus { get; set; }
        public string QuantityRemaining { get; set; }
        public string Status { get; set; }
    }
}