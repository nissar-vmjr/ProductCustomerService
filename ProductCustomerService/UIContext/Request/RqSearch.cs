using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RqSearch:PaginationContext
    {
        public long? ID { get; set; }

        public string startDate { get; set; }

        public string endDate { get; set; }
    }
}