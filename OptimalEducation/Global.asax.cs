using Implimentation.CQRS;
using Interfaces.CQRS;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.DAL.Models;
using OptimalEducation.Implementation.Logic.Characterizers;
using OptimalEducation.Implementation.Logic.MulticriterialAnalysis;
using OptimalEducation.Interfaces.Logic.Characterizers;
using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis;
using OptimalEducation.Models;
using SimpleInjector;
using SimpleInjector.Extensions;
using SimpleInjector.Integration.Web.Mvc;
using System.Data.Entity;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;


namespace OptimalEducation
{
    public class MvcApplication : System.Web.HttpApplication
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
            RegisterAllQueries(Container);
            RegisterAllCommands(Container);
            RegisterAllLogic(Container);

            //Entity Framework contexts
            Container.RegisterPerWebRequest<IOptimalEducationDbContext, OptimalEducationDbContext>();
            Container.RegisterPerWebRequest<ApplicationDbContext, ApplicationDbContext>();
            Container.RegisterPerWebRequest<DbContext, ApplicationDbContext>();
            //userManager classes
            Container.RegisterPerWebRequest<IUserStore<ApplicationUser>>(() =>
                new UserStore<ApplicationUser>(Container.GetInstance<DbContext>()));
            Container.RegisterPerWebRequest<IApplicationUserManager, ApplicationUserManager>();


            Container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            //Container.RegisterMvcIntegratedFilterProvider();

            Container.Verify();
        }

        private void RegisterAllCommands(Container container)
        {
            Container.RegisterManyForOpenGeneric(
                typeof(ICommand<>),
                typeof(IOptimalEducationDbContext).Assembly);
        }

        private void RegisterAllQueries(Container container)
        {
            Container.RegisterManyForOpenGeneric(
                typeof(IQuery<,>),
                typeof(IOptimalEducationDbContext).Assembly);
        }

        private void RegisterAllLogic(Container container)
        {
            var implementationCharacterizerAssembly = typeof(EducationLineDistanceRecomendator).Assembly;
            Container.RegisterManyForOpenGeneric(
                typeof(IDistanceRecomendator<,>),
                Lifestyle.Singleton,
                implementationCharacterizerAssembly);

            Container.RegisterManyForOpenGeneric(
                typeof(ISummator<>),
                //Lifestyle, ???
                implementationCharacterizerAssembly);

            Container.RegisterManyForOpenGeneric(
                typeof(IIdealResult<>),
                //Lifestyle, ???         
                implementationCharacterizerAssembly);

            //По умолчанию будет возвращаться singleton класс со стандартными опциями вычисления
            //В отдельных классах в коде может присутсвовать ручное инстанцирование
            Container.RegisterSingle<ICharacterizer<Entrant>, EntrantCharacterizer>();
            Container.RegisterSingle<ICharacterizer<EducationLine>, EducationLineCharacterizer>();

            Container.RegisterSingle<IPreferenceRelationCalculator, PreferenceRelationCalculator>();
            Container.RegisterSingle<IVectorCriteriaRecalculator, VectorCriteriaRecalculator>();
            Container.RegisterSingle<IParretoCalculator, ParretoCalculator>();

            Container.RegisterSingle<IMulticriterialAnalysisRecomendator, MulticriterialAnalysis>();

            Container.RegisterSingle<IEducationCharacteristicNamesHelper, EducationCharacteristicNamesHelper>();
        }
    }
}
