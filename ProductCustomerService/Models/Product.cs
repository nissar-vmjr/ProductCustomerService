using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductCustomerService.Models
{
    public class Product
    {
        public long ProductID { get; set; }

        [MaxLength(50)]
        public string ProductName { get; set; }

        [MaxLength(50)]
        public string ProductCode { get; set; }

        public double VATPercentage { get; set; }

        [MaxLength(100)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDateTime { get; set; }
    }
}