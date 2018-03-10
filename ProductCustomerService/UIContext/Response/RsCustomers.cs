using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RsCustomers:PaginationContext
    {
        public RsCustomers()
        {
            customers = new List<customer>();
        }
        public List<customer> customers { get; set; }

        public double allCustomersBalance { get; set; }
    }
}