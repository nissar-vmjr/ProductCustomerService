using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RsBalanceTransaction:BaseContext
    {
        public RsBalanceTransaction()
        {
            balanceTransaction = new balanceTransaction();
        }
        public balanceTransaction balanceTransaction { get; set; }

    }
}