using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RsProductTransactions:PaginationContext
    {
        public RsProductTransactions()
        {
            transactions = new List<productTransaction>();
        }
        public List<productTransaction> transactions { get; set; }
    }
}