using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using OptimalEducation.Models;

namespace OptimalEducation.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IApplicationUserManager _userManager;

        public HomeController(IApplicationUserManager applicationUserManager)
        {
            _userManager = applicationUserManager;
        }

        public async Task<ActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();

                if (_userManager.Users.SingleOrDefault(p => p.Id == userId) != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(userId);
                    if (userRoles.Any(role => role == Role.Entrant))
                        return RedirectToAction("Index", "Orientation", new {area = "EntrantUser"});
                    if (userRoles.Any(role => role == Role.Faculty))
                        return RedirectToAction("Index", "Info", new {area = "FacultyUser"});
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
    }
}