using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RqProductTransactionsSearch:PaginationContext
    {
        public long? productID { get; set; }

        public long? subProductID { get; set; }

        public string startDate { get; set; }

        public string endDate { get; set; }
    }
}