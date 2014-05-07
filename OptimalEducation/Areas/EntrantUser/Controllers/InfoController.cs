using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.DAL.Models;
using OptimalEducation.Logic.Characterizer;
using OptimalEducation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OptimalEducation.Logic.MulticriterialAnalysis;
using OptimalEducation.Logic.AnalyticHierarchyProcess;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles=Role.Entrant)]
	public class InfoController : Controller
	{
		private OptimalEducationDbContext db = new OptimalEducationDbContext();
		private ApplicationDbContext dbIdentity = new ApplicationDbContext();

		public UserManager<ApplicationUser> UserManager { get; private set; }

		public InfoController()
		{
			UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbIdentity));
		}
		public InfoController(UserManager<ApplicationUser> userManager)
		{
			UserManager = userManager;
		}

		// GET: /EntrantUser/Info/
		public async Task<ActionResult> Index()
		{
            //Отобразить характеристики текущего пользователя
			var entrantId = await GetEntrantId();
			var entrant = await db.Entrants
				.FindAsync(entrantId);

            var entrantCharacteristics = new EntrantCharacterizer(entrant, new EntrantCalculationOptions()).CalculateNormSum();
            
            //Отобразить рекомендуемые учебные направления

            //По методу сравнения расстояний мд характеристиками
            var educationLines = await db.EducationLines
                .Where(p => p.Actual == true)
                .ToListAsync();
            ViewBag.RecomendationForEntrant = DistanceCharacterisiticRecomendator.GetRecomendationForEntrant(entrant, educationLines);
            //По методу многокритериального анализа
            var multicriterialAnalyzer = new MulticriterialAnalysis(entrant,educationLines);
            ViewBag.MulticriterialRecomendations = multicriterialAnalyzer.Calculate();
            //По МАИ
            //var AHPAnalyzer=new AHPUser(entrantId,)
            //ViewBag.APHRecomendations = 
            return View(entrantCharacteristics);
		}

		private async Task<int> GetEntrantId()
		{
			var currentUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			var entrantClaim = currentUser.Claims.FirstOrDefault(p => p.ClaimType == MyClaimTypes.EntityUserId);
			var entrantId = int.Parse(entrantClaim.ClaimValue);
			return entrantId;
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
				dbIdentity.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}