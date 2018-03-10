using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RsNotifications:BaseContext
    {
        public RsNotifications()
        {
            chequeAlerts = new List<chequeAlert>();
            customerAlerts = new List<customerAlert>();
            subProductAlerts = new List<subProductAlert>();
        }
        public List<chequeAlert> chequeAlerts { get; set; }
        public List<customerAlert> customerAlerts { get; set; }

        public List<subProductAlert> subProductAlerts { get; set; }

    }
}