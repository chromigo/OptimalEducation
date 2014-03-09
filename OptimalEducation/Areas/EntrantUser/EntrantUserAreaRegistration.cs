using System.Web.Mvc;

namespace OptimalEducation.Areas.EntrantUser
{
    public class EntrantUserAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "EntrantUser";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "EntrantUser_default",
                "EntrantUser/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}