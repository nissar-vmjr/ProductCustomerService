using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RsProducts:BaseContext
    {
        public RsProducts()
        {
            products = new List<product>();
        }

        public List<product> products { get; set; }
    }
}