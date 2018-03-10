using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RqProduct:BaseContext
    {
        public string productName { get; set; }

        public double vatPercentage { get; set; }
    }
}