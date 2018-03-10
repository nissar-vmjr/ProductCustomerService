using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RsSuppliers : BaseContext
    {
        public RsSuppliers()
        {
            suppliers = new List<supplier>();
        }
        public List<supplier> suppliers { get; set; }

        public double totalBalanceAmount {get;set;}
    }
}