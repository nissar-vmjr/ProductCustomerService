using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductCustomerService.Models
{
    public class Customer
    {
        public long CustomerID { get; set; }

        [MaxLength(100)]
        public string CustomerFirstName { get; set; }

        [MaxLength(100)]
        public string CustomerLastName { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string Keyword { get; set; }

        [MaxLength(2000)]
        public string Address { get; set; }  

        public double? InitialBalance { get; set; }
        public double NetOutstandingBalance { get; set; }

        public double MaxLimitAllowed { get; set; }

        public int? MaxWaitingDays { get; set; }

        [MaxLength(50)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        [MaxLength(50)]
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDateTime { get; set; }
        public virtual ICollection<CustomerContact> CustomerContacts { get; set; }
    }
}