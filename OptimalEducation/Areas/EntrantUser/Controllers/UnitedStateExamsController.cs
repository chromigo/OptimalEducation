using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OptimalEducation.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.Logic.Characterizer;
using OptimalEducation.DAL.Models;


namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles=Role.Entrant)]
	public class UnitedStateExamsController : Controller
	{
        private OptimalEducationDbContext db = new OptimalEducationDbContext();
        private ApplicationDbContext dbIdentity = new ApplicationDbContext();
		public UserManager<ApplicationUser> UserManager { get; private set; }

		public UnitedStateExamsController()
		{
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbIdentity));
		}

		public UnitedStateExamsController(UserManager<ApplicationUser> userManager)
		{
			UserManager = userManager;
		}
		// GET: /EntrantUser/UnitedStateExams/
		public async Task<ActionResult> Index()
		{
			var examList = await GetUserExamsAsync();
			return View(examList);
		}

		private async Task<List<UnitedStateExam>> GetUserExamsAsync()
		{
            var entrantId = await GetEntrantId();
			var unitedstateexams = db.UnitedStateExams
				.Include(u => u.Discipline)
				.Include(u => u.Entrant)
                .Where(p => p.EntrantId == entrantId);
			var examList = await unitedstateexams.ToListAsync();
			return examList;
		}

		// POST: /EntrantUser/UnitedStateExams/Index
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Index([Bind(Include = "Result")] IEnumerable<UnitedStateExam> unitedStateExams)
		{
			var examList = await GetUserExamsAsync();
			//Порядок не должен меняться, поэтому обновляем данные так
			for (int i = 0; i < examList.Count; i++)
			{
				examList[i].Result = unitedStateExams.ElementAt(i).Result;
			}
			unitedStateExams = examList;//обновляем список новыми значениями(для передачи во View, в случае ошибки)

			//Проверка 
			if (ModelState.IsValid)
			{
				foreach (var exam in unitedStateExams)
				{
					db.Entry(exam).State = EntityState.Modified;
				}
				await db.SaveChangesAsync();
                return RedirectToAction("Index");
			}

			return View(unitedStateExams);
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
