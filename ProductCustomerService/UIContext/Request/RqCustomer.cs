using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RqCustomer:BaseContext
    {
        //public RqCustomer()
        //{
        //    customer = new customer();
        //}
        public customer customer { get; set; }

        public bool isAllowNumberRepeat { get; set; }
    }
}