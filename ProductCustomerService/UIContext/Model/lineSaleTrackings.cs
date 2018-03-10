using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class lineSaleTracking
    {
        public long lineSaleID { get; set; }

        public long lineSaleTrackingID { get; set; }

        public double amountPaid { get; set; }

        public string paymentDate { get; set; }

        public string comments { get; set; }

        public double balanceAmount { get; set; }

        public bool isDeletable { get; set; }

        public bool isBold { get; set; }
    }
}