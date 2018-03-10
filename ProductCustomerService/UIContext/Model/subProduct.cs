using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class subProduct
    {
        public long? subProductID { get; set; }
       
        public string subProductName { get; set; }

        public long quantityAvailable { get; set; }

        public double markedPrice { get; set; }

        public product product { get; set; }
    }
}