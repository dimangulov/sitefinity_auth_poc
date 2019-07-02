using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Authentication;
using Telerik.Sitefinity.Services;
using WebApplication5.Custom;

namespace WebApplication5
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            Bootstrapper.Bootstrapped += SystemManager_ApplicationStart;
        }
        private void SystemManager_ApplicationStart(object sender, EventArgs e)
        {
            ObjectFactory.Container.RegisterType<AuthenticationProvidersInitializer, CustomAuthenticationProvidersInitializer>(new ContainerControlledLifetimeManager());
        }
    }
}
