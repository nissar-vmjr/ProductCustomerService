using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProductCustomerService.Models
{
    public class Supplier
    {
        public int SupplierID { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public DateTime? LastUpdatedDatetime { get; set; }

        [MaxLength(100)]
        public string LastUpdatedBy { get; set; }

        public double BalanceAmount { get; set; }

        public double InitialBalance { get; set; }

        public bool DisplayInDropdown { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? CreatedDateTime { get; set; }
    }
}