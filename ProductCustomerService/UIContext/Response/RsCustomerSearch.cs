using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RsCustomerSearch:BaseContext
    {
        public RsCustomerSearch()
        {
            customers = new List<customer>();
        }

        public List<customer> customers;
    }
}