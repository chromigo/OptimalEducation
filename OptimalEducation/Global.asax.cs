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

            //http://www.lightinject.net/ используемый ioc контейнер с руководством по использованию
            RegisterIoC();

            //другой вариант для работы с ioc контейнерами. создать класс-адаптер для конкретного ioc контейнера
            //ControllerBuilder.Current.SetControllerFactory(new LightInjectControllerFactory()); 
        }

        private void RegisterIoC()
        {
            var container = new ServiceContainer();
            container.ScopeManagerProvider = new PerLogicalCallContextScopeManagerProvider();
            container.RegisterControllers();
            //register other services

            //contexts
            container.Register<OptimalEducationDbContext, OptimalEducationDbContext>(new PerRequestLifeTime());
            container.Register<ApplicationDbContext, ApplicationDbContext>(new PerRequestLifeTime());
            //userManager classes
            container.Register<IUserStore<ApplicationUser>>(
                factory => new UserStore<ApplicationUser>(factory.GetInstance<ApplicationDbContext>()), new PerRequestLifeTime());
            container.Register<IApplicationUserManager, ApplicationUserManager>(new PerRequestLifeTime());

            container.EnableMvc();
        }
    }
}
