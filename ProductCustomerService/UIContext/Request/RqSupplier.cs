using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RqSupplier:BaseContext
    {
        public int supplierID { get; set; }

        public double amount { get; set; }

        public string comments { get; set; }

        public bool isPayment { get; set; }
    }
}