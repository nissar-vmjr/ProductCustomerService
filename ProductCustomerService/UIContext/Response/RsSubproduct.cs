using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RsSubproduct:BaseContext
    {
        public RsSubproduct()
        {
            subProduct = new subProduct();
        }        

        public subProduct subProduct { get; set; }
    }
}