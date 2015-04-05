using System.Data.Entity;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Implementation.CQRS;
using Interfaces.CQRS;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.DAL;
using OptimalEducation.DAL.Models;
using OptimalEducation.Helpers;
using OptimalEducation.Implementation.Logic.Characterizers;
using OptimalEducation.Implementation.Logic.DistanceRecomendator;
using OptimalEducation.Implementation.Logic.MulticriterialAnalysis;
using OptimalEducation.Interfaces.Logic.Characterizers;
using OptimalEducation.Interfaces.Logic.DistanceRecomendator;
using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis;
using OptimalEducation.Models;
using SimpleInjector;
using SimpleInjector.Extensions;
using SimpleInjector.Integration.Web.Mvc;

namespace OptimalEducation
{
    public class MvcApplication : HttpApplication
    {
        public static Container Container { get; private set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //https://simpleinjector.codeplex.com/ используемый ioc контейнер с руководством по использованию
            RegisterIoC();
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(Container));
        }

        private void RegisterIoC()
        {
            Container = new Container();

            //Регистриуем основные компоненты
            Container.RegisterPerWebRequest<IQueryBuilder, QueryBuilder>();
            Container.RegisterPerWebRequest<ICommandBuilder, CommandBuilder>();
            RegisterAllQueries();
            RegisterAllCommands();
            RegisterAllLogic();

            //Entity Framework contexts
            Container.RegisterPerWebRequest<IOptimalEducationDbContext, OptimalEducationDbContext>();
            Container.RegisterPerWebRequest<ApplicationDbContext, ApplicationDbContext>();
            Container.RegisterPerWebRequest<DbContext, ApplicationDbContext>();
            //userManager classes
            Container.RegisterPerWebRequest<IUserStore<ApplicationUser>>(() =>
                new UserStore<ApplicationUser>(Container.GetInstance<DbContext>()));
            Container.RegisterPerWebRequest<IApplicationUserManager, ApplicationUserManager>();
            Container.RegisterPerWebRequest<IInfoExtractor, InfoExtractor>();
            Container.RegisterPerWebRequest<IIdealResult, IdealEntrantResult>();

            Container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            //Container.RegisterMvcIntegratedFilterProvider();

            Container.Verify();
        }

        private void RegisterAllCommands()
        {
            Container.RegisterManyForOpenGeneric(
                typeof (ICommand<>),
                typeof (IOptimalEducationDbContext).Assembly);
        }

        private void RegisterAllQueries()
        {
            Container.RegisterManyForOpenGeneric(
                typeof (IQuery<,>),
                typeof (IOptimalEducationDbContext).Assembly);
        }

        private void RegisterAllLogic()
        {
            Container.RegisterManyForOpenGeneric(
                typeof (IDistanceRecomendator<,>),
                Lifestyle.Singleton,
                typeof (EducationLineDistanceRecomendator).Assembly);

            Container.RegisterManyForOpenGeneric(
                typeof (ISummator<>),
                //Lifestyle, ???
                typeof (EducationLineSummator).Assembly);

            //По умолчанию будет возвращаться singleton класс со стандартными опциями вычисления
            //В отдельных классах в коде может присутсвовать ручное инстанцирование
            Container.RegisterSingle<ICharacterizer<Entrant>, EntrantCharacterizer>();
            Container.RegisterSingle<ICharacterizer<EducationLine>, EducationLineCharacterizer>();

            Container.RegisterSingle<IPreferenceRelationCalculator, PreferenceRelationCalculator>();

            Container.RegisterSingle<IMulticriterialAnalysisRecomendator, MulticriterialAnalysisRecomendator>();

            Container.RegisterSingle<IEducationCharacteristicNamesHelper, EducationCharacteristicNamesHelper>();
        }
    }
}