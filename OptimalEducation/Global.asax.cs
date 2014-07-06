using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DryIoc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.DAL.Models;
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

            //https://bitbucket.org/dadhi/dryioc/wiki/Home используемый ioc контейнер с руководством по использованию
            RegisterIoC();
        }

        private void RegisterIoC()
        {
            var dryIoC = new Container();
            //TODO регистриция контролеров

            dryIoC.RegisterInstance(dryIoC);
            dryIoC.Register<IDependencyResolver, DependencyResolver>(Reuse.Singleton);
            dryIoC.Register<IQueryBuilder, QueryBuilder>();
            dryIoC.Register<ICommandBuilder, CommandBuilder>();

            //register other services
            //TODO Проверить время жизни
            //contexts
            dryIoC.Register<IOptimalEducationDbContext, OptimalEducationDbContext>();
            dryIoC.Register<ApplicationDbContext, ApplicationDbContext>();
            dryIoC.Register<DbContext, ApplicationDbContext>();
            //userManager classes
            dryIoC.Register<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(Reuse.Transient, new GetConstructor(t => t.GetConstructor(new Type[] { typeof(DbContext) })));
            dryIoC.Register<IApplicationUserManager, ApplicationUserManager>();

            RegisterAllQueries(dryIoC);

            RegisterAllCommands(dryIoC);

            RegisterControllers(dryIoC);
        }

        private void RegisterAllCommands(Container dryIoC)
        {
            var pluginAssembly = typeof(IOptimalEducationDbContext).Assembly;
            var commandInterfaceType = typeof (ICommand<>);

            var commandTypes = pluginAssembly
                .GetTypes()
                .Where(t =>
                    t.IsPublic &&
                    !t.IsAbstract &&
                    t.GetImplementedTypes().Contains(commandInterfaceType, new TypeEqualityComparer<Type>()));
            foreach (var type in commandTypes)
            {
                var typeInterface = type.GetImplementedTypes().Single(p => p.Name == commandInterfaceType.Name);
                dryIoC.Register(typeInterface, type, Reuse.Transient);
            }
        }

        private void RegisterAllQueries(Container dryIoC)
        {
            var pluginAssembly = typeof(IOptimalEducationDbContext).Assembly;
            var queryInterfaceType = typeof (IQuery<,>);

            var queryTypes = pluginAssembly
                .GetTypes()
                .Where(t =>
                    t.IsPublic &&
                    !t.IsAbstract &&
                    t.GetImplementedTypes().Contains(queryInterfaceType, new TypeEqualityComparer<Type>()));
            foreach (var type in queryTypes)
            {
                var typeInterface = type.GetImplementedTypes().Single(p => p.Name == queryInterfaceType.Name);
                dryIoC.Register(typeInterface, type, Reuse.Transient);
            }
        }

        private void RegisterControllers(Container dryIoC)
        {
            var controllerAssembly = typeof (MvcApplication).Assembly;

            var controllerTypes = controllerAssembly
                .GetTypes()
                .Where(t =>
                    t.IsPublic &&
                    !t.IsAbstract &&
                    t.GetImplementedTypes().Contains(typeof (IController)));

            foreach (var controllerType in controllerTypes)
                dryIoC.Register(controllerType, Reuse.Transient);

            var controllerFactory = new DryIoCControllerFactory(dryIoC);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
            //DependencyResolver.SetResolver(controllerFactory);
        }
    }
    public class DependencyResolver : IDependencyResolver
    {
        private readonly Container _dryIoC;
        public DependencyResolver(Container dryIoC)
        {
            _dryIoC = dryIoC;
        }
        public object GetService(Type serviceType)
        {
            return _dryIoC.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
           return null;
        }
    }

    public class DryIoCControllerFactory : DefaultControllerFactory
    {
        private readonly Container _dryIoC;

        public DryIoCControllerFactory(Container dryIoC)
        {
            _dryIoC = dryIoC;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(404, string.Format("The controller for path '{0}' could not be found.", requestContext.HttpContext.Request.Path));
            }

            return (IController) _dryIoC.Resolve(controllerType);
        }

        public override void ReleaseController(IController controller)
        {
            var disposable = controller as IDisposable;
            if (disposable != null) disposable.Dispose();
            else
            {
                throw new Exception("Controller have not IDisposable interface");
            }
        }
    }

    public class TypeEqualityComparer<T> : IEqualityComparer<T> where T:Type
    {

        public bool Equals(T x, T y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }

}
