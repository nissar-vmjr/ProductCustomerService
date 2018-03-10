using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class customerContact
    {
        public long? customerContactID { get; set; }
        public int contactTypeID { get; set; }
        public string countryCode { get; set; }  
        public string cityCode { get; set; }  
        public string contactNumber { get; set; }
    }
}