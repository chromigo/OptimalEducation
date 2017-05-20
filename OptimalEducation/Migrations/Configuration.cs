using System.Data.Entity.Migrations;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.Identity;
using OptimalEducation.Models;

namespace OptimalEducation.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        private ApplicationUserManager _userManager;

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            CreateRoles(context);

            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            CreateAdminUser();
            CreateEntrantUsers();
            CreateFacultyUsers();
        }

        private void CreateRoles(ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new
                RoleStore<IdentityRole>(context));
            //Create Role Admin if it does not exist
            if (!roleManager.RoleExists(Role.Admin))
            {
                roleManager.Create(new IdentityRole(Role.Admin));
            }
            if (!roleManager.RoleExists(Role.Entrant))
            {
                roleManager.Create(new IdentityRole(Role.Entrant));
            }
            if (!roleManager.RoleExists(Role.Faculty))
            {
                roleManager.Create(new IdentityRole(Role.Faculty));
            }
        }

        private void CreateAdminUser()
        {
            var user = new ApplicationUser();
            user.UserName = "administrator";
            const string password = "administrator";

            if (_userManager.FindByName(user.UserName) == null)
            {
                var identityResult = _userManager.Create(user, password);

                if (identityResult.Succeeded)
                {
                    _userManager.AddToRole(user.Id, Role.Admin);
                }
            }
        }

        private void CreateEntrantUsers()
        {
            var user = new ApplicationUser();
            user.UserName = "entrant";
            const string password = "entrant";

            if (_userManager.FindByName(user.UserName) == null)
            {
                var identityResult = _userManager.Create(user, password);

                if (identityResult.Succeeded)
                {
                    _userManager.AddToRole(user.Id, Role.Entrant);
                    _userManager.AddClaim(user.Id, new Claim(MyClaimTypes.EntityUserId, "1"));
                }
            }
        }

        private void CreateFacultyUsers()
        {
            var user = new ApplicationUser();
            user.UserName = "faculty";
            const string password = "faculty";

            if (_userManager.FindByName(user.UserName) == null)
            {
                var identityResult = _userManager.Create(user, password);

                if (identityResult.Succeeded)
                {
                    _userManager.AddToRole(user.Id, Role.Faculty);
                    _userManager.AddClaim(user.Id, new Claim(MyClaimTypes.EntityUserId, "1"));
                }
            }
        }
    }
}