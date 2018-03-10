using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ProductCustomerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace ProductCustomerService.Controllers
{
    public class BaseController : ApiController
    {
        //public string UserLogin { get; private set; }
        //public BaseController()
        //{
        //    if (User != null && User.Identity != null)
        //    {

        //        var identity = User.Identity as ClaimsIdentity;
        //        UserLogin = identity != null ? identity.Name : string.Empty;
        //    }
        //}

        private ApplicationUser _member;

        public ApplicationUserManager UserManager
        {
            get { return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
        }

        public string UserLogin
        {
            get
            {
                var user = UserManager.FindByName(User.Identity.Name);
                return user.UserName;
            }
        }

        public ApplicationUser UserRecord
        {
            get
            {
                if (_member != null)
                {
                    return _member;
                }
                _member = UserManager.FindByEmail(Thread.CurrentPrincipal.Identity.Name);
                return _member;
            }
            set { _member = value; }
        }
    }
}
