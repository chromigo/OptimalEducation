using Microsoft.Owin;
using OptimalEducation.DAL.Models;
using OptimalEducation.Migrations;
using OptimalEducation.Models;
using Owin;
using System.Data.Entity;
using System.Linq;

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
