using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class balanceTransaction
    {
        public long? balanceTransactionID { get; set; }

        public long customerID { get; set; }

        public double incomingAmount { get; set; }

        public double outgoingAmount { get; set; }

        //public bool isDebt { get; set; }

        public int transactionTypeID { get; set; }

        public string transactionDate { get; set; }

        public string chequeDate { get; set; }

        public string chequeDepositedDate { get; set; }


        public string chequeActionDate { get; set; }

       
        public string chequeNumber { get; set; }

       
        public string chequeCustomerName { get; set; }
        
        public string chequeIssuerBank { get; set; }
        public int? chequeStatusID { get; set; }

       
        public string onlineReferernceID { get; set; }

    
        public string comments { get; set; }

        public double? outstandingBalance { get; set; }

        public bool? isAmountPaid { get; set; }

        //public bool? isChequeFailed { get; set; }

        public string chequeFailureComments { get; set; }

        public bool isEditable { get; set; }

        public bool isDeletable { get; set; }
        public customer customer { get; set; }
    }
}