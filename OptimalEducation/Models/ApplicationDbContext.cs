using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Security.Claims;

namespace OptimalEducation.Models
{

    public static class MyClaimTypes
    {
        public const string EntityUserId = "EntityUserId";
    }
    public static class Role
    {
        public const string Admin = "Admin";
        public const string Entrant = "Entrant";
        public const string Faculty = "Faculty";
    }
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }
}