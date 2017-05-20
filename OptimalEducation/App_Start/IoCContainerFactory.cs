using System.Data.Entity;
using System.Reflection;
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
using SimpleInjector.Integration.Web;

namespace OptimalEducation
{
    public class IoCContainerFactory
    {
        private static readonly Assembly DalAssembly = typeof(IOptimalEducationDbContext).Assembly;
        private static readonly Assembly DistanceRecomendatorAssembly = typeof(EducationLineDistanceRecomendator).Assembly;
        private static readonly Assembly CharacterisersAssembly = typeof(EducationLineSummator).Assembly;

        public static Container Create()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            RegisterCQRS(container);
            RegisterAllLogic(container);
            RegisterDbContexts(container);
            RegisterIdentity(container);
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterMvcIntegratedFilterProvider();

            container.Verify();
            return container;
        }

        private static void RegisterIdentity(Container container)
        {
            container.Register<IUserStore<ApplicationUser>>(() =>
                new UserStore<ApplicationUser>(container.GetInstance<DbContext>()), Lifestyle.Scoped);
            container.Register<IApplicationUserManager, ApplicationUserManager>(Lifestyle.Scoped);

            container.Register<IInfoExtractor, InfoExtractor>(Lifestyle.Scoped);
            container.Register<IIdealResult, IdealEntrantResult>(Lifestyle.Scoped);
        }

        private static void RegisterCQRS(Container container)
        {
            container.Register<IQueryBuilder, QueryBuilder>(Lifestyle.Scoped);
            container.Register<ICommandBuilder, CommandBuilder>(Lifestyle.Scoped);
            container.Register(typeof(IQuery<,>), new[] { DalAssembly }, Lifestyle.Scoped);
            container.Register(typeof(ICommand<>), new[] { DalAssembly }, Lifestyle.Scoped);
        }

        private static void RegisterDbContexts(Container container)
        {
            container.Register<IOptimalEducationDbContext, OptimalEducationDbContext>(Lifestyle.Scoped);
            container.Register<ApplicationDbContext, ApplicationDbContext>(Lifestyle.Scoped);
            container.Register<DbContext, ApplicationDbContext>(Lifestyle.Scoped);
        }

        private static void RegisterAllLogic(Container container)
        {
            container.Register(typeof(IDistanceRecomendator<,>), new[] { DistanceRecomendatorAssembly }, Lifestyle.Scoped);
            container.Register(typeof(ISummator<>), new[] { CharacterisersAssembly }, Lifestyle.Scoped);

            //ѕо умолчанию будет возвращатьс€ singleton класс со стандартными опци€ми вычислени€
            //¬ отдельных классах в коде может присутсвовать ручное инстанцирование
            container.Register(typeof(ICharacterizer<>), new[] { CharacterisersAssembly }, Lifestyle.Scoped);

            container.Register<IPreferenceRelationCalculator, PreferenceRelationCalculator>(Lifestyle.Scoped);
            container.Register<IMulticriterialAnalysisRecomendator, MulticriterialAnalysisRecomendator>(Lifestyle.Scoped);
            container.Register<IEducationCharacteristicNamesHelper, EducationCharacteristicNamesHelper>(Lifestyle.Scoped);
        }
    }
}