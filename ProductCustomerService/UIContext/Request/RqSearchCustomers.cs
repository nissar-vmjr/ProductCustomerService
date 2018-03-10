using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RqSearchCustomers:PaginationContext
    {
        public string keyword { get; set; }
    }
}