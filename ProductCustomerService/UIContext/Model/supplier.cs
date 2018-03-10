using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class supplier
    {
        public int supplierID { get; set; }

        public string supplierName { get; set; }

        public double balanceAmount { get; set; }

        public string lastUpdatedDate { get; set; }
    }
}