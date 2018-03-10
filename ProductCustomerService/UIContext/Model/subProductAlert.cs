using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class subProductAlert
    {
        public long subProductID { get; set; }

        public string subProductName { get; set; }

        public long quantityAvailable { get; set; }

        public long? productID { get; set; }

        public string productName { get; set; }

        public long? minAlertQuantity { get; set; }

        public string lastTransactionDate { get; set; }

    }
}