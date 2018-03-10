using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public abstract class BaseContext
    {
        public BaseContext()
        {
            RuleViolations = new List<RuleViolation>();
        }  
        public ResponseStatus ResponseStatus { get; set; }

        public string ResponseMessage { get; set; }

        public string WebAPIName { get; set; }

        public string Url { get; set; }

        public string UserLogin { get; set; }

        public List<RuleViolation> RuleViolations { get; set; }
    }

    public enum ResponseStatus
    {
        Success = 1,
        Failed = 2,
        Warning = 3,  
    }
}