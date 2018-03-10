using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.BusinessLayer
{
    public class BusinessHelper
    {
        public static string FormatName(string firstName, string lastName, string keyword)
        {
            string result = null;
            result = firstName + (!string.IsNullOrWhiteSpace(lastName) ? " " + lastName + (!string.IsNullOrWhiteSpace(keyword) ? "(" + keyword + ")" : string.Empty) : (!string.IsNullOrWhiteSpace(keyword) ? "(" + keyword + ")" : string.Empty));
            return result;
        }

        public static DateTime GetLastUpdatedDateTime(DateTime? createdDatetime,DateTime? lastUpdateddatetime)
        {
            if(lastUpdateddatetime.HasValue)
            {
                return lastUpdateddatetime.Value;
            }
            else
            {
                return createdDatetime.Value;
            }
        }

        public static DateTime AddDays(DateTime date,int days)
        {
            return date.AddDays(days);
        }
    }
}