using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.DAL.Models;
using OptimalEducation.Logic.Clusterizer;
using OptimalEducation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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
			var entrantId = await GetEntrantId();
			var entrant = await db.Entrants
				.FindAsync(entrantId);
			var clusterizer = new EntrantClusterizer(entrant);
			return View(clusterizer.Cluster);
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