using System.Web.Mvc;
using OptimalEducation.Models;

namespace OptimalEducation.Areas.FacultyUser.Controllers
{
    [Authorize(Roles=Role.Faculty)]
    public class InfoController : Controller
    {
        //
        // GET: /FacultyUser/Info/
        public ActionResult Index()
        {
            return View();
        }
	}
}