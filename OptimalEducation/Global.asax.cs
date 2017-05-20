using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;

namespace OptimalEducation
{
    public class MvcApplication : HttpApplication
    {
        private Container _container;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            _container = IoCContainerFactory.Create();
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(_container));
        }

        protected void Application_End()
        {
            _container.Dispose();
        }
    }
}