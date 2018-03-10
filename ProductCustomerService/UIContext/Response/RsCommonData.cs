using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RsCommonData:BaseContext
    {
        public RsCommonData()
        {
            contactTypes = new List<lookup>();
            notificationTypes = new List<lookup>();
            transactionTypes = new List<lookup>();
            chequeStatusTypes = new List<lookup>();
            warehouses = new List<lookup>();
            suppliers = new List<lookup>();
            lineSaleSuppliers = new List<lookup>();
        }

        public List<lookup> contactTypes { get; set; }
        public List<lookup> notificationTypes { get; set; }

        public List<lookup> transactionTypes { get; set; }

        public List<lookup> chequeStatusTypes { get; set; }

        public List<lookup> warehouses { get; set; }

        public List<lookup> suppliers { get; set; }

        public List<lookup> lineSaleSuppliers { get; set; }
    }
}