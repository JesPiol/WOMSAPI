using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace COCAAPI.Models
{
    public class PIDetail
    {
        public string PPINNo { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceAmount { get; set; }
        public string Company { get; set; }
        public string PONo { get; set; }

        // ---------------- added JBC

        // ----------- Vendor Ledger Entry
        public string CVReferenceNo { get; set; }
        // ----------- Vendor Ledger Entry

        public string PIPaymentTerms { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceAmountIncVAT { get; set; }
        public string WorkPenalties { get; set; }
        public string AmountPayable { get; set; }
        public string PPINDate { get; set; }

        // ----------- Check Ledger Entry
        public string CVDate { get; set; }
        public string CVStatus { get; set; }
        public string CVAmount { get; set; }
        // ----------- Check Ledger Entry


        // ---------------- added JBC
    }
}