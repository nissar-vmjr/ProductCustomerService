using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductCustomerService.Models
{
    public class Log
    {
        public long LogID { get; set; }

        [MaxLength(1000)]
        public string Message { get; set; }

        [MaxLength(2000)]
        public string InnerException { get; set; }

        [MaxLength(4000)]
        public string StackTrace { get; set; }

        [MaxLength(100)]
        public string LoggedUser { get; set; }

        public DateTime LoggedDateTime { get; set; }
    }
}