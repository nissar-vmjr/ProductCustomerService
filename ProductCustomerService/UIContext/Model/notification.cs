using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class notification
    {
        public long notificationID { get; set; }

        public int notificationTypeID { get; set; }

        public string message { get; set; }

        public bool isActive { get; set; }

        public bool isPriorityAlert { get; set; }

    }
}