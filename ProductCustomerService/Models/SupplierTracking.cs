using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductCustomerService.Models
{
    public class SupplierTracking
    {
        public long SupplierTrackingID { get; set; }

        public int SupplierID { get; set; }

        public double PaidAmount { get; set; }

        public double BorrowedAmount { get; set; }

        [MaxLength(2000)]
        public string comments { get; set; }

        public DateTime? CreatedDatetime { get; set; }

        [MaxLength(100)]
        public string CreatedBy { get; set; }

        public virtual Supplier Supplier { get; set; }
    }
}