using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RqChangeChequeStatus:BaseContext
    {
        public long? balanceTransactionID { get; set; }
        public string chequeDepositedDate { get; set; }
        public string chequeActionDate { get; set; }
        public int chequeStatusID { get; set; }
        public string chequeFailureComments { get; set; }
        public string chequeClosureComments { get; set; }
    }
}