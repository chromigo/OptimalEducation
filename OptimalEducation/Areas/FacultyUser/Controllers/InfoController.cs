using OptimalEducation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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