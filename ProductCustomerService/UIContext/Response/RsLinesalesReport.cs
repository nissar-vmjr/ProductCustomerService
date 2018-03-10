using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RsLinesalesReport:BaseContext
    {
        public RsLinesalesReport()
        {
            closedLineSales = new List<lineSale>();
            pendingLineSales = new List<lineSale>();
            trackedLineSales = new List<lineSale>();
        }

        public List<lineSale> closedLineSales { get; set; }
        public List<lineSale> pendingLineSales { get; set; }

        public List<lineSale> trackedLineSales { get; set; }

        public double totalClosedAmount { get; set; }

        public double totalNewPendingAmount { get; set; }

        public double oldBillPaymentAmount { get; set; }
    }
}