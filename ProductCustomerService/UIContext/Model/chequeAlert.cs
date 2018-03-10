using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class chequeAlert
    {
        public long balanceTransactionID { get; set; }

        public long customerID { get; set; }

        public string customerFirstName { get; set; }
        public string customerLastName { get; set; }

        public string customerKeyword { get; set; }
        public int chequeStatusID { get; set; }

        public string chequeDate { get; set; }
        public string chequeDepositDate { get; set; }

        public string chequeActionDate { get; set; }

        public string chequeNumber { get; set; }

        public string comments { get; set; }

        public string chequeCustomerName { get; set; }

        public string chequeIssuerBank { get; set; }
        public string chequeFailureComments { get; set; }

        public bool isHighPriority { get; set; }

        public double chequeAmount { get; set; }

    }
}