using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RsCustomer:BaseContext
    {
        public RsCustomer()
        {
            customer = new customer();
        }

        public customer customer { get; set; }

        
    }
}