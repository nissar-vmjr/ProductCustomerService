using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RsLineSaleTrackings:BaseContext
    {
        public RsLineSaleTrackings()
        {
            lineSaleTrackings = new List<lineSaleTracking>();
        }
        public List<lineSaleTracking> lineSaleTrackings { get; set; }
    }
}