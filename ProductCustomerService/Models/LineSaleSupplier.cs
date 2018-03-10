using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductCustomerService.Models
{
    public class LineSaleSupplier
    {
        public int LineSaleSupplierID { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }
    }
}