namespace OptimalEducation.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using OptimalEducation.Models;

    public struct Role
    {
        public const string Admin = "Admin";
        public const string Entrant = "Entrant";
        public const string Faculty = "Faculty"; 
    }

    internal sealed class Configuration : DbMigrationsConfiguration<OptimalEducation.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(OptimalEducation.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.
            var cities = new List<City>
            {
                new City { Name = "Москва", Prestige = 90 },
                new City { Name = "Санкт-Петербург", Prestige = 80 },
                new City { Name = "Екатеринбург", Prestige = 60 }
            };
            cities.ForEach(s => context.Cities.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            var higherEducationInstitutions = new List<HigherEducationInstitution>
            {
                new HigherEducationInstitution { Name = "МГУ", Prestige = 90, CityId=cities.Single(p=>p.Name=="Москва").Id, Type=HigherEducationInstitutionType.University},
                new HigherEducationInstitution { Name = "СПбГУ", Prestige = 80, CityId=cities.Single(p=>p.Name=="Санкт-Петербург").Id, Type=HigherEducationInstitutionType.University  },
                new HigherEducationInstitution { Name = "УРГУ", Prestige = 60, CityId=cities.Single(p=>p.Name=="Екатеринбург").Id, Type=HigherEducationInstitutionType.University }
            };
            higherEducationInstitutions.ForEach(s => context.HigherEducationInstitutions.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            var faculties = new List<Faculty>
            {
                new Faculty { Name = "Кафедра МГУ1", Prestige = 90, HigherEducationInstitutionId=higherEducationInstitutions.Single(p=>p.Name=="МГУ").Id},
                new Faculty { Name = "Кафедра СПбГУ1", Prestige = 80, HigherEducationInstitutionId=higherEducationInstitutions.Single(p=>p.Name=="СПбГУ").Id },
                new Faculty { Name = "Кафедра УРГУ1", Prestige = 60, HigherEducationInstitutionId=higherEducationInstitutions.Single(p=>p.Name=="УРГУ").Id}
            };
            faculties.ForEach(s => context.Faculties.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            var disciplines = new List<ExamDiscipline>
            {
                new ExamDiscipline { Name = "Русский язык"},
                new ExamDiscipline { Name = "Математика"},
                new ExamDiscipline { Name = "Информатика"},
                new ExamDiscipline { Name = "Физика"},
                new ExamDiscipline { Name = "Химия"},
                new ExamDiscipline { Name = "Английский язык"},
            };
            disciplines.ForEach(s => context.ExamDisciplines.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            CreateRoles(context);

            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new
    UserStore<ApplicationUser>(context));

            CreateAdminUser(UserManager);
            
            CreateEntrantUsers(UserManager);
            var user =UserManager.FindByName("Entrant1");
            foreach (var disc in context.ExamDisciplines)
            {
                if (!user.Entrant.UnitedStateExams.Any(p => p.Discipline == disc && p.EntrantId == user.Entrant.Id))
                user.Entrant.UnitedStateExams.Add(
                    new UnitedStateExam
                       {
                           Discipline = disc,
                           Entrant = user.Entrant,
                       });
            }
            context.SaveChanges();

            CreateFacultyUsers(UserManager, faculties);
        }

        private void CreateRoles(OptimalEducation.Models.ApplicationDbContext context)
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

        private void CreateAdminUser(UserManager<ApplicationUser> UserManager)
        {
            var user = new ApplicationUser();
            user.UserName = "Administrator";
            const string password = "password";

            if (UserManager.FindByName(user.UserName) == null)
            {
                var identityResult = UserManager.Create(user, password);

                if (identityResult.Succeeded)
                {
                    UserManager.AddToRole(user.Id, Role.Admin);
                }
            }
        }
        private void CreateEntrantUsers(UserManager<ApplicationUser> UserManager)
        {
            var user = new ApplicationUser();
            user.UserName = "Entrant1";
            const string password = "password";

            user.Entrant = new Entrant();
            if (UserManager.FindByName(user.UserName) == null)
            {
                var identityResult = UserManager.Create(user, password);

                if (identityResult.Succeeded)
                {
                    UserManager.AddToRole(user.Id, Role.Entrant);
                }
            }
        }
        private void CreateFacultyUsers(UserManager<ApplicationUser> UserManager, List<Faculty> higherEducationInstitutions)
        {
            var user = new ApplicationUser();
            user.UserName = "Faculty1";
            user.FacultyId = higherEducationInstitutions.First().Id;
            const string password = "password";

            if (UserManager.FindByName(user.UserName) == null)
            {
                var identityResult = UserManager.Create(user, password);

                if (identityResult.Succeeded)
                {
                    UserManager.AddToRole(user.Id, Role.Faculty);
                }
            }
        }
        private void CreateUniqueUserAndAddRole(UserManager<ApplicationUser> UserManager, ApplicationUser user, string password)
        {
            if (UserManager.FindByName(user.UserName) == null)
            {
                var identityResult = UserManager.Create(user, password);

                if (identityResult.Succeeded)
                {
                    UserManager.AddToRole(user.Id, Role.Admin);
                }
            }

        }
    }
}
