using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RsProductTransaction:BaseContext
    {
        public RsProductTransaction()
        {
            transaction = new productTransaction();
        }

        public productTransaction transaction { get; set; }
    }
}