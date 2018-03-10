using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class customerAlert
    {
        public long customerID { get; set; }

        public string customerFirstName { get; set; }
        public string customerLastName { get; set; }
        public string keyword { get; set; }

        public customerContact primaryContact { get; set; }

        public customerContact secondaryContact { get; set; }
        public double netOutstandingBalance { get; set; }

        public double maxLimitAmount { get; set; }

        public int? maxWaitingDays { get; set; }

        public string lastTransactionDate { get; set; }
    }
}