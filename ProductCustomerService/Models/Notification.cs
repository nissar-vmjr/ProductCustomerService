using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace ProductCustomerService.Models
{
    public class Notification
    {
        public long NotificationID { get; set; }

        public int NotificationTypeID { get; set; }

        [MaxLength(2000)]
        public string Message { get; set; }

        public bool IsActive { get; set; }

        public bool IsPriorityAlert { get; set; }

        public virtual NotificationType NotificationType { get; set; }
    }
}