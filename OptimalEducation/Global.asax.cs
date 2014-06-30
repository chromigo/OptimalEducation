using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using LightInject;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.DAL.Models;
using OptimalEducation.Models;

namespace OptimalEducation
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            RegisterIoC();
        }

        private void RegisterIoC()
        {
            var container = new ServiceContainer();
            container.RegisterControllers();
            //register other services
            container.Register<OptimalEducationDbContext, OptimalEducationDbContext>(new PerRequestLifeTime());
            container.Register<ApplicationDbContext, ApplicationDbContext>(new PerRequestLifeTime());

            container.Register<IUserStore<ApplicationUser>>(
                factory => new UserStore<ApplicationUser>(factory.GetInstance<ApplicationDbContext>()), new PerRequestLifeTime());
            container.Register<UserManager<ApplicationUser>, UserManager<ApplicationUser>>(new PerRequestLifeTime());

            container.EnableMvc();
        }
    }
}
