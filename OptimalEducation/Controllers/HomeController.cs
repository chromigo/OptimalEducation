using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using OptimalEducation.Models;
using StackExchange.Profiling;

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
            var profiler = MiniProfiler.Current; // it's ok if this is null
            using (profiler.Step("Set page title"))
            {
                ViewBag.Title = "Home Page";
            }
            using (profiler.Step("Doing complex stuff"))
            {
                using (profiler.Step("Step A"))
                { // something more interesting here
                    Thread.Sleep(100);
                }
                using (profiler.Step("Step B"))
                { // and here
                    Thread.Sleep(250);
                }
            }

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();

                if (_userManager.Users.SingleOrDefault(p => p.Id == userId) != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(userId);
                    if (userRoles.Any(role => role == Role.Admin))
                        return RedirectToAction("Index", "EducationLines", new {area = "Admin"});
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