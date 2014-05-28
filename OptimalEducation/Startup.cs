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

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<OptimalEducationDbContext, OptimalEducation.DAL.Migrations.Configuration>());

            //Обратимся к бд, чтобы проинициализировать
            using (var apContext = new ApplicationDbContext())
            {
                var t = apContext.Users.Count();
            }
            using (var apContext = new OptimalEducationDbContext())
            {
                var t = apContext.Entrants.Count();
            }
        }
    }
}
