using System.Web.Mvc;

namespace OptimalEducation.Areas.FacultyUser
{
    public class FacultyUserAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "FacultyUser";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "FacultyUser_default",
                "FacultyUser/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}