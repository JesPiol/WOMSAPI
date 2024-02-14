using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace COCAAPI.Models
{
    public class ViolationViewModel
    {
       public string IdNo { get; set; }
       public string ViolationType { get; set; }
       public string ViolationDescription { get; set; }
       public string ViolationDate { get; set; }
       public string NoOfOffense { get; set; }
       public string MonetaryPenalty { get; set; }
       public string ProcessDate { get; set; }
       public string PONo { get; set; }
    }
}