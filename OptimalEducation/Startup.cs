using Microsoft.Owin;
using OptimalEducation;
using Owin;

[assembly: OwinStartup(typeof (Startup))]

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