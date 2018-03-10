using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RsSubProducts:BaseContext
    {
        public RsSubProducts()
        {
            subProducts = new List<subProduct>();
        }

        public List<subProduct> subProducts { get; set; }
    }
}