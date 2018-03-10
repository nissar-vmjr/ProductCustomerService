using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class RuleViolation
    {
        public string Message { get; set; }
        public RuleViolation(string message)
        {
            Message = message;
        }
    }
}