using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;
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
using System.Diagnostics;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles=Role.Entrant)]
	public class OrientationController : Controller
	{
	    private readonly OptimalEducationDbContext _dbContext;
	    private readonly UserManager<ApplicationUser> _userManager;

		public OrientationController(OptimalEducationDbContext dbContext, UserManager<ApplicationUser> userManager)
		{
		    _dbContext = dbContext;
		    _userManager = userManager;
		}

	    // GET: /EntrantUser/Orientation/
		public async Task<ActionResult> Index()
		{
			var entrantId = await GetEntrantId();
		    var query = new GetEntrantForCharacterizerByIdQuery(entrantId, _dbContext);
		    var entrant = await query.Execute();

            //Предпочтения пользователя по предметам и пр.
            var entrantCharacteristics = new EntrantCharacterizer(entrant, new EntrantCalculationOptions()).CalculateNormSum();//add true for complicated method
            ViewBag.Preferences = entrantCharacteristics;

			return View();
		}

		private async Task<int> GetEntrantId()
		{
			var currentUser = await _userManager.FindByIdAsync(User.Identity.GetUserId());
			var entrantClaim = currentUser.Claims.FirstOrDefault(p => p.ClaimType == MyClaimTypes.EntityUserId);
			var entrantId = int.Parse(entrantClaim.ClaimValue);
			return entrantId;
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
                //dbContext.Dispose();
                //UserManager.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}