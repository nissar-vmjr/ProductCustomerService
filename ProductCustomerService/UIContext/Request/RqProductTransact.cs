using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RqProductTransact:BaseContext
    {
        //public RqProductTransact()
        //{
        //    productTransaction = new productTransaction();
        //    //balanceTransaction = new balanceTransaction();
        //}
        public productTransaction productTransaction { get; set; }

        //public bool isIncludeCustomer { get; set; }

        //public balanceTransaction balanceTransaction { get; set; }
    }
}