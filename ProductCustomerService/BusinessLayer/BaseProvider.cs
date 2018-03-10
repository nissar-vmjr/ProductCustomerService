using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProductCustomerService.UIContext;

namespace ProductCustomerService.BusinessLayer
{
    public class BaseProvider
    {
        public BaseProvider()
        {

        }

        public void ThrowCustomException(string message)
        {
            throw new Exception(message)
            {
                Source = "Custom"
            };
        }

        public void ThrowWarningException(string message)
        {
            throw new Exception(message)
            {
                Source = "Warning"
            };
        }

        public void GetExceptionResponse(BaseContext response,string error, Exception exception)
        {
            if (exception.Source == "Warning")
            {
                response.ResponseStatus = ResponseStatus.Warning;
                response.ResponseMessage= exception.Source == "Warning" ? exception.Message : error;
            }
            else
            {
                response.ResponseStatus = ResponseStatus.Failed;
                response.ResponseMessage = exception.Source == "Custom" ? exception.Message : error;
            }

            LogHelper.Log(exception);
            
          
        }
    }
}