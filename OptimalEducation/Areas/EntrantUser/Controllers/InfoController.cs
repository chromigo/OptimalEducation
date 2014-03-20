using OptimalEducation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
    [Authorize(Roles=Role.Entrant)]
    public class InfoController : Controller
    {
        //
        // GET: /EntrantUser/Info/
        public ActionResult Index()
        {
            return View();
        }
	}
}