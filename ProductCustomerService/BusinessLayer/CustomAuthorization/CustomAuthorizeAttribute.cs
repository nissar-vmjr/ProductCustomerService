using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ProductCustomerService.BusinessLayer
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomAuthorizeAttribute: AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (actionContext != null && actionContext.RequestContext != null && actionContext.RequestContext.Principal != null && actionContext.RequestContext.Principal.Identity != null)
            {
                var identity = actionContext.RequestContext.Principal.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    using (ProdCustContext context = new ProdCustContext())
                    {
                        var user = context.AppUsers.Where(x => x.UserName.ToLower() == identity.Name.ToLower()).FirstOrDefault();
                        if (user == null)
                        {
                            return false;
                        }
                    }
                }
            }
                return true;
        }
    }
}