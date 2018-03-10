using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RqBalanceTransaction:BaseContext
    {       
        public long customerID { get; set; }
        public balanceTransaction balanceTransaction { get; set; }
    }
}