using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using OptimalEducation.Helpers;
using OptimalEducation.Helpers.Webpack;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;

namespace OptimalEducation
{
    public class MvcApplication : HttpApplication
    {
        private Container _container;
        private FileSystemWatcher _webpackConfigWatcher;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            _container = IoCContainerFactory.Create();
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(_container));

            var applicationBasePath = AppDomain.CurrentDomain.BaseDirectory;
            var webpackAssetsFilePath = $@"{applicationBasePath}\frontend\bundles\assets.json";

            WebpackConfigWatcher.Initialize(webpackAssetsFilePath);
        }

        protected void Application_End()
        {
            _container.Dispose();
        }
    }
}