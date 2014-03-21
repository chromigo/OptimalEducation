using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OptimalEducation.DAL.Models;
using OptimalEducation.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles = Role.Entrant)]
	public class SchoolController : Controller
	{
		private OptimalEducationDbContext db = new OptimalEducationDbContext();
		private ApplicationDbContext dbIdentity = new ApplicationDbContext();
		public UserManager<ApplicationUser> UserManager { get; private set; }
		public SchoolController()
		{
			UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbIdentity));
		}
		public SchoolController(UserManager<ApplicationUser> userManager)
		{
			UserManager = userManager;
		}
		// GET: /EntrantUser/School/
		public async Task<ActionResult> Index()
		{
			var entrantId = await GetEntrantId();
			var participationinSchools = db.ParticipationInSchools
				.Include(p => p.Entrants)
				.Include(p => p.School)
				.Where(p => p.EntrantsId == entrantId);
			return View(await participationinSchools.ToListAsync());
		}

		// GET: /EntrantUser/School/Details/5
		public async Task<ActionResult> Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var entrantId = await GetEntrantId();
			ParticipationInSchool participationinSchool = await db.ParticipationInSchools
				.Where(p => p.EntrantsId == entrantId)
				.FirstOrDefaultAsync(p => p.Id == id);
			if (participationinSchool == null)
			{
				return HttpNotFound();
			}
			return View(participationinSchool);
		}

		// GET: /EntrantUser/School/Create
		public ActionResult Create()
		{
			ViewBag.SchoolId = new SelectList(db.Schools, "Id", "Name");
			return View();
		}

		// POST: /EntrantUser/School/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "YearPeriod,SchoolId")] ParticipationInSchool participationinSchool)
		{
			participationinSchool.EntrantsId = await GetEntrantId();
			if (ModelState.IsValid)
			{
				db.ParticipationInSchools.Add(participationinSchool);
				await db.SaveChangesAsync();
				return RedirectToAction("Index");
			}

			ViewBag.SchoolId = new SelectList(db.Schools, "Id", "Name", participationinSchool.SchoolId);
			return View(participationinSchool);
		}

		// GET: /EntrantUser/School/Edit/5
		public async Task<ActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var entrantId = await GetEntrantId();
			ParticipationInSchool participationinSchool = await db.ParticipationInSchools
				.Where(p=>p.EntrantsId==entrantId)
				.FirstOrDefaultAsync(p=>p.Id==id);
			if (participationinSchool == null)
			{
				return HttpNotFound();
			}
			return View(participationinSchool);
		}

		// POST: /EntrantUser/School/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,YearPeriod")] ParticipationInSchool participationinSchool)
		{
			//var entrantId = await GetEntrantId();
			//participationinSchool.EntrantId = entrantId;

			if (ModelState.IsValid)
			{
				var dbPartOlymp = await db.ParticipationInSchools.FindAsync(participationinSchool.Id);
                dbPartOlymp.YearPeriod = participationinSchool.YearPeriod;
				await db.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			return View(participationinSchool);
		}

		// GET: /EntrantUser/School/Delete/5
		public async Task<ActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var entrantId = await GetEntrantId();
			ParticipationInSchool participationinSchool = await db.ParticipationInSchools
				.Where(p => p.EntrantsId == entrantId)
				.FirstOrDefaultAsync(p => p.Id == id);
			if (participationinSchool == null)
			{
				return HttpNotFound();
			}
			return View(participationinSchool);
		}

		// POST: /EntrantUser/School/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			var entrantId = await GetEntrantId();
			ParticipationInSchool participationinSchool = await db.ParticipationInSchools
				.Where(p => p.EntrantsId == entrantId)
				.FirstOrDefaultAsync(p => p.Id == id);
			db.ParticipationInSchools.Remove(participationinSchool);
			await db.SaveChangesAsync();
			return RedirectToAction("Index");
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
			}
			base.Dispose(disposing);
		}
	}
}
