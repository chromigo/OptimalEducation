using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OptimalEducation.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            if(User.Identity.IsAuthenticated)
            {
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

                var userId = User.Identity.GetUserId();
                
                if(UserManager.Users.SingleOrDefault(p => p.Id == userId)!=null)
                {
                    var userRoles = await UserManager.GetRolesAsync(userId);
                    if (userRoles.Any(role => role == Role.Admin))
                        return RedirectToLocal("/Admin/Entrant/");
                    if (userRoles.Any(role => role == Role.Entrant))
                        return RedirectToLocal("/EntrantUser/Info/");
                    if (userRoles.Any(role => role == Role.Faculty))
                        return RedirectToLocal("/FacultyUser/Info/");
                }
            }
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}