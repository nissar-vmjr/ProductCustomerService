using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductCustomerService.Models
{
    public class AppUser
    {
        public int AppUserID { get; set; }

        [MaxLength(200)]
        public string UserName { get; set; }
    }
}