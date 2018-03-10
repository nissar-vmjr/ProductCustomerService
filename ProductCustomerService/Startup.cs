using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Web.Http;


[assembly: OwinStartup(typeof(ProductCustomerService.Startup))]

namespace ProductCustomerService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           
            ConfigureAuth(app);
            //WebApiConfig.Register(config);
           

            //GlobalConfiguration.Configure(WebApiConfig.Register);

            //app.UseWebApi(config);

        }
    }
}
