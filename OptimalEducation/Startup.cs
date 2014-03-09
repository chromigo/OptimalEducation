using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OptimalEducation.Startup))]
namespace OptimalEducation
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
