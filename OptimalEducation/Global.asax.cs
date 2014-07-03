using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using LightInject;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Models;
using CQRS;

namespace OptimalEducation
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private readonly ServiceContainer lightInject = new ServiceContainer();
        
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
            lightInject.ScopeManagerProvider = new PerLogicalCallContextScopeManagerProvider();
            lightInject.RegisterControllers();
            //register other services

            //contexts
            lightInject.Register<IOptimalEducationDbContext, OptimalEducationDbContext>(new PerRequestLifeTime());
            lightInject.Register<ApplicationDbContext, ApplicationDbContext>(new PerRequestLifeTime());
            //userManager classes
            lightInject.Register<IUserStore<ApplicationUser>>(
                factory => new UserStore<ApplicationUser>(factory.GetInstance<ApplicationDbContext>()), new PerRequestLifeTime());
            lightInject.Register<IApplicationUserManager, ApplicationUserManager>(new PerRequestLifeTime());

            lightInject.Register<IQuery<TestCriteria, IEnumerable<ParticipationInOlympiad>>, GetAllPartQuery>();
            lightInject.Register<IQueryBuilder>(factory=>new QueryBuilder(lightInject));//Передаем в явном виде сам наш инжектор
            lightInject.EnableMvc();
        }
    }
}
