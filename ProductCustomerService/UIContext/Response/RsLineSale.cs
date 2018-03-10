using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RsLineSale:BaseContext
    {
        public RsLineSale()
        {
            lineSales = new List<lineSale>();
        }

        public List<lineSale> lineSales { get; set; }

        public double totalPendingAmount { get; set; }
    }
}