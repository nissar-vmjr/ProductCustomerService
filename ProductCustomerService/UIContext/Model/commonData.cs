using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class commonData
    {
        public commonData()
        {
            contactTypes = new List<lookup>();
            transactionTypes = new List<lookup>();
            notificationTypes = new List<lookup>();
        }

        public List<lookup> contactTypes { get; set; }

        public List<lookup> transactionTypes { get; set; }

        public List<lookup> notificationTypes { get; set; }
    }
}