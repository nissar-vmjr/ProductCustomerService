using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class PaginationContext:BaseContext
    {
        public int pageSize { get; set; }

        public int pageIndex { get; set; }

        public bool isDescending { get; set; }

        public int totalRecords { get; set; }
    }
}