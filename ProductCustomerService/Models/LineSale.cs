using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProductCustomerService.Models
{
    public class LineSale
    {
        public long LineSaleID { get; set; }

        public int? LineSaleSupplierID { get; set; }

        [MaxLength(200)]
        public string SupplierName { get; set; }

        public DateTime ItemsDeliveredDate { get; set; }

        public double TotalAmount { get; set; }

        public double BalanceAmount { get; set; }

        [MaxLength(2000)]
        public string Comments { get; set; }

        public bool IsTrackingRequired { get; set; }

        [MaxLength(50)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        [MaxLength(50)]
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDateTime { get; set; }

        //public DateTime? PaymentDate { get; set; }

   
        public virtual ICollection<LineSaleTracking> LineSaleTrackings { get; set; }
        public virtual LineSaleSupplier LineSaleSupplier { get; set; }
    }
}