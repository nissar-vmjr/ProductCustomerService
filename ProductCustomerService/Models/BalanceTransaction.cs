using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProductCustomerService.Models
{
    public class BalanceTransaction
    {
        public long BalanceTransactionID { get; set; }

        public long CustomerID { get; set; }

        public double IncomingAmount { get; set; }

        public double OutgoingAmount { get; set; }

        //public bool IsDebt { get; set; }

        public int TransactionTypeID { get; set; }

        public DateTime TransactionDate { get; set; }

        public DateTime? ChequeDate { get; set; }

        public DateTime? ChequeDepositedDate { get; set; }

        public DateTime? ChequeActionDate { get; set; }

        [MaxLength(200)]
        public string ChequeNumber { get; set; }

        [MaxLength(100)]
        public string ChequeCustomerName { get; set; }
        [MaxLength(100)]
        public string ChequeIssuerBank { get; set; }

        public int? ChequeStatusID { get; set; }
        //public bool? IsChequePassed { get; set; }

        [MaxLength(100)]
        public string OnlineReferernceID { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        public double OutstandingBalance { get; set; }

        //public bool? IsChequeFailed { get; set; }

        [MaxLength(2000)]
        public string ChequeFailureComments { get; set; }

        [MaxLength(2000)]
        public string ChequeClosureComments { get; set; }
        [MaxLength(50)]
        public string CreatedBy { get; set; }
      
        public DateTime? CreatedDateTime { get; set; }

        [MaxLength(50)]
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDateTime { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual TransactionType TransactionType { get; set; }

        public virtual ChequeStatus ChequeStatus { get; set; }
    }
}