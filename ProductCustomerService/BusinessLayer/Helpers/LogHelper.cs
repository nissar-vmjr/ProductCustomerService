using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProductCustomerService.Utilities;

namespace ProductCustomerService.BusinessLayer
{
    public static class LogHelper

    {

        private static LogService logger = null;

        public static void Log(Exception exception)
        {
            logger = new LogService();
            logger.LogError(exception);

        }

    }
}