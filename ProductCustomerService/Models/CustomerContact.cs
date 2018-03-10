using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductCustomerService.Models
{
    public class CustomerContact
    {
        public long CustomerContactID { get; set; }

        public int ContactTypeID { get; set; }
        public long CustomerID { get; set; }

        [MaxLength(50)]
        public string CountryCode { get; set; }

        [MaxLength(50)]
        public string CityCode { get; set; }

        [MaxLength(50)]
        public string ContactNumber { get; set; }

        public bool IsPrimaryContact { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ContactType ContactType { get; set; }
    }
}