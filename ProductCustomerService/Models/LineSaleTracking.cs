using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProductCustomerService.Models
{
    public class LineSaleTracking
    {
        public long LineSaleTrackingID { get; set; }
        public long LineSaleID { get; set; }

        public DateTime PaymentDate { get; set; }
        public double AmountPaid { get; set; }

        public double BalanceAmount { get; set; }

        [MaxLength(1000)]
        public string Comments { get; set; }
        public bool IsBalanceSettled { get; set; }

        [MaxLength(50)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public virtual LineSale LineSale { get; set; }
    }
}