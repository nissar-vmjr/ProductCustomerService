using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class product
    {
        public long? productID { get; set; }

       
        public string productName { get; set; }

     
        public string productCode { get; set; }

        public double? vatPercentage { get; set; }
    }
}