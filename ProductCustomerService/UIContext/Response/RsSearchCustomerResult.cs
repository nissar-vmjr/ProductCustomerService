using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RsSearchCustomerResult:PaginationContext
    {
        public RsSearchCustomerResult()
        {
            transactions = new List<balanceTransaction>();
        }

        public List<balanceTransaction> transactions { get; set; }

        public double currentBalanceAmount { get; set; }

    }
}