using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class lineSale
    {

        public lineSale()
        {
            lineSaleTrackings = new List<lineSaleTracking>();
        }
        public long lineSaleID { get; set; }
        public int? supplierID { get; set; }

        public double billedAmount { get; set; }
  
        public string supplierName { get; set; }

        public string itemsDeliveredDate { get; set; }

        public double balanceAmount { get; set; }

        public string comments { get; set; }

        public string lastPaymentDate { get; set; }

        public bool allowPayment { get; set; }

        public bool hasTrackings { get; set; }

        public bool isDeletable { get; set; }

        public List<lineSaleTracking> lineSaleTrackings { get; set; }

    }
}