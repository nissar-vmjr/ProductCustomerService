using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RsCustomerKeywordSearch: BaseContext
    {
        public RsCustomerKeywordSearch()
        {
            customers = new List<customerSearch>();
        }

        public List<customerSearch> customers { get; set; }
    }
}