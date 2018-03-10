using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductCustomerService.Models
{
    public class SubProduct
    {
        public long SubProductID { get; set; }

        public long ProductID { get; set; }

        [MaxLength(100)]
        public string SubProductName { get; set; }

        public long QuantityAvailable { get; set; }

        public double MarkedPrice { get; set; }

        public long? MinAlertQuantity { get; set; }

        public bool isActive { get; set; }

        public DateTime? LastUpdatedDatetime { get; set; }

        [MaxLength(100)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        [MaxLength(100)]
        public string LastUpdatedBy { get; set; }
        public virtual Product Product { get; set; }
    }
}