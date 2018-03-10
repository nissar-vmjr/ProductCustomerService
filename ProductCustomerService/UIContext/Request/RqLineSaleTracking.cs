using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RqLineSaleTracking:BaseContext
    {
        public long lineSaleID { get; set; }

        public double amountPaid { get; set; }

        public string comments { get; set; }
        public bool isBalanceSettled { get; set; }

        public string paymentDate { get; set; }
    }
}