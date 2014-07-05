using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using LightInject;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.DAL.Commands;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Models;
using CQRS;

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
            var lightInject = new DependencyResolver();
            lightInject.ScopeManagerProvider = new PerLogicalCallContextScopeManagerProvider();
            lightInject.RegisterControllers();
            //register other services

            lightInject.Register<IDependencyResolver>(factory => lightInject);
            //contexts
            lightInject.Register<IOptimalEducationDbContext, OptimalEducationDbContext>(new PerRequestLifeTime());
            lightInject.Register<ApplicationDbContext, ApplicationDbContext>(new PerRequestLifeTime());
            //userManager classes
            lightInject.Register<IUserStore<ApplicationUser>>(
                factory => new UserStore<ApplicationUser>(factory.GetInstance<ApplicationDbContext>()), new PerRequestLifeTime());
            lightInject.Register<IApplicationUserManager, ApplicationUserManager>(new PerRequestLifeTime());

            lightInject.Register<IQuery<GetAllParticipationInOlympiadCriterion, Task<IEnumerable<ParticipationInOlympiad>>>, GetAllParticipationInOlympiadOfEntrantQuery>();
            lightInject.Register<IQuery<GetAllOlympiadsCriterion, Task<IEnumerable<Olympiad>>>, GetAllOlympiadsQuery>();
            lightInject.Register<IQuery<GetCurrentParticipationInOlympiadCriterion, Task<ParticipationInOlympiad>>, GetCurrentParticipationInOlympiadQuery>();
            lightInject.Register<IQuery<GetEntrantCriterion, Task<Entrant>>, GetEntrantQuery>();

            lightInject.Register<IQueryBuilder>(factory=>new QueryBuilder(lightInject));//Передаем в явном виде сам наш инжектор
            
            lightInject.Register<ICommand<AddParticipationInOlympiadContext>, AddParticipationInOlympiadCommand>();
            lightInject.Register<ICommand<RemoveParticipationInOlympiadContext>, RemoveParticipationInOlympiadCommand>();
            lightInject.Register<ICommand<UpdateParticipationInOlympiadResultContext>, UpdateParticipationInOlympiadResultCommand>();
            lightInject.Register<ICommandBuilder>(factory => new CommandBuilder(lightInject));//Передаем в явном виде сам наш инжектор

            lightInject.EnableMvc();
        }
    }

    public class DependencyResolver : ServiceContainer, IDependencyResolver
    {
        //Реализации этого интерфейса должны осуществлять делегирование в базовый контейнер внедрения зависимостей,
        //чтобы предоставить зарегистрированную службу для запрошенного типа.
        //При отсутствии зарегистрированных служб запрошенного типа ASP.NET MVC ожидает от реализации этого интерфейса возврата null из GetService
        //или пустой коллекции из GetServices.
        public object GetService(Type serviceType)
        {
            return base.GetInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return base.GetAllInstances(serviceType);
        }
    }
}
