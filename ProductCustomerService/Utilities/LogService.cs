using ProductCustomerService.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;


namespace ProductCustomerService.Utilities
{
    public class LogService 
    {

        private readonly string filePath = ConfigurationManager.AppSettings["LogFilePath"];
        private readonly string applicationUrl = ConfigurationManager.AppSettings["ApplicationURL"];

        private void Log(Exception exception)
        {


            //using (StreamWriter writer = new StreamWriter(applicationUrl+filePath, true))
            //{
            //    writer.Write(GenerateCustomMessage(exception));
            //    writer.WriteLine("Date :" + DateTime.Now.ToString());
            //    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            //}
            try
            {
                var log = new Log();
                GenerateDBLogRecord(log, exception);
                using (ProdCustContext context = new ProdCustContext())
                {
                    context.Logs.Add(log);
                    context.SaveChanges();
                }
            }
            catch(Exception errorException)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.Write(GenerateCustomMessage(errorException));
                    writer.Write(Environment.NewLine + Environment.NewLine);
                    writer.Write(GenerateCustomMessage(exception));
                    
                    writer.WriteLine("Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }
        }

        /// <summary>
        /// Generates the custom message.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        private string GenerateCustomMessage(Exception ex)
        {
            StringBuilder errorMsg = new StringBuilder();
            errorMsg.Append(Environment.NewLine);
            string userLogIn = string.Empty;
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null)
            {
                userLogIn = System.Web.HttpContext.Current.User.Identity.Name;
            }
            errorMsg.Append(string.Format("User Login : {0}{1}", userLogIn, Environment.NewLine));
            errorMsg.Append(string.Format("Exception Message : {0}{1}", ex.Message, Environment.NewLine));
            errorMsg.Append(string.Format("Inner Exception : {0}{1}", ex.InnerException, Environment.NewLine));
            errorMsg.Append(string.Format("Stack Trace : {0}{1}", ex.StackTrace, Environment.NewLine));

            if (ex.Data.Count > 0)
            {
                ICollection values = ex.Data.Values;
                errorMsg.Append(string.Format("Additional Messages : {0}", Environment.NewLine));
                foreach (var value in values)
                {
                    errorMsg.Append(value.ToString() + Environment.NewLine);
                }
            }
            errorMsg.Append(ex.StackTrace);
            return errorMsg.ToString() + Environment.NewLine;
        }

        private void GenerateDBLogRecord(Log log,Exception exception)
        {
            log.Message = exception.Message;
            log.InnerException = exception.InnerException!=null?exception.InnerException.Message:string.Empty;
            log.StackTrace = exception.StackTrace;
            log.LoggedDateTime = DateTime.Now;
        }

     

        public void LogError(Exception exception)
        {
            Log(exception);
        }

        public string PrepareInfoTraceMessage(string message)
        {
            return string.Format("{0}[Info]: {1}-{2}", Environment.NewLine, DateTime.Now, message);
        }
    }
}