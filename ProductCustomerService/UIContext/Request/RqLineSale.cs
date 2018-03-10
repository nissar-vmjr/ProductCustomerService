using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RqLineSale:BaseContext
    {
        public int supplierID { get; set; }
        public string supplierName { get; set; }
        public double totalAmount { get; set; }
        public string itemRecievedDate { get; set; }

        public string comments { get; set; }
        public bool isTrackingRequired { get; set; }

        public double? amountPaid { get; set; }
    }
}