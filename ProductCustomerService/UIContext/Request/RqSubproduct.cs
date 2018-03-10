using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RqSubproduct:BaseContext
    {
        public long productID { get; set; }       
        public string subProductName { get; set; }

        public long quantityAvailable { get; set; }

        public double markedPrice { get; set; }

        public long? minAlertQuantity { get; set; }
    }
}