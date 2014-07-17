namespace OptimalEducation.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using OptimalEducation.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Security.Claims;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
        ApplicationUserManager UserManager;
        protected override void Seed(ApplicationDbContext context)
        {
            CreateRoles(context);

            UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            CreateAdminUser();
            CreateEntrantUsers();
            CreateFacultyUsers();
        }
        private void CreateRoles(ApplicationDbContext context)
        {
            RoleManager<IdentityRole> RoleManager = new RoleManager<IdentityRole>(new
        RoleStore<IdentityRole>(context));
            //Create Role Admin if it does not exist
            if (!RoleManager.RoleExists(Role.Admin))
            {
                var roleresult = RoleManager.Create(new IdentityRole(Role.Admin));
            }
            if (!RoleManager.RoleExists(Role.Entrant))
            {
                var roleresult = RoleManager.Create(new IdentityRole(Role.Entrant));
            }
            if (!RoleManager.RoleExists(Role.Faculty))
            {
                var roleresult = RoleManager.Create(new IdentityRole(Role.Faculty));
            }
        }
        private void CreateAdminUser()
        {
            var user = new ApplicationUser();
            user.UserName = "administrator";
            const string password = "administrator";

            if (UserManager.FindByName(user.UserName) == null)
            {
                var identityResult = UserManager.Create(user, password);

                if (identityResult.Succeeded)
                {
                    UserManager.AddToRole(user.Id, Role.Admin);
                }
            }
        }
        private void CreateEntrantUsers()
        {
            var user = new ApplicationUser();
            user.UserName = "entrant";
            const string password = "entrant";

            if (UserManager.FindByName(user.UserName) == null)
            {
                var identityResult = UserManager.Create(user, password);

                if (identityResult.Succeeded)
                {
                    UserManager.AddToRole(user.Id, Role.Entrant);
                    UserManager.AddClaim(user.Id, new Claim(MyClaimTypes.EntityUserId, "1"));
                }
            }
        }
        private void CreateFacultyUsers()
        {
            var user = new ApplicationUser();
            user.UserName = "faculty";
            const string password = "faculty";

            if (UserManager.FindByName(user.UserName) == null)
            {
                var identityResult = UserManager.Create(user, password);

                if (identityResult.Succeeded)
                {
                    UserManager.AddToRole(user.Id, Role.Faculty);
                    UserManager.AddClaim(user.Id, new Claim(MyClaimTypes.EntityUserId, "1"));
                }
            }
        }
    }
}
