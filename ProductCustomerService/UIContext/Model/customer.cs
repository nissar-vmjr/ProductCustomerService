using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class customer
    {
        public customer()
        {
            //primaryContact = new customerContact();
            //secondaryContact = new customerContact();
        }
        public long? customerID { get; set; }

        public string customerFirstName { get; set; }
        public string customerLastName { get; set; }
        public string address { get; set; }

        public string email { get; set; }

        public string keyword { get; set; }

        public customerContact primaryContact { get; set; }

        public customerContact secondaryContact { get; set; }

        public double netOutstandingBalance { get; set; }

        public double maxLimitAmount { get; set; }

        public int? maxWaitingDays { get; set; }

        public string latestTransactionDate { get; set; }

        public double totalOutgoingAmount { get; set; }
    }
    
}