using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RqSupplierNew:BaseContext
    {
        public string supplierName { get; set; }

        public double balanceAmount { get; set; }

        public bool displayInDropdown { get; set; }
    }
}