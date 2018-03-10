using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCustomerService.Models
{
    public class NotificationType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NotificationTypeID { get; set; }

        [MaxLength(100)]
        public string NotificationTypeName { get; set; }
    }
}