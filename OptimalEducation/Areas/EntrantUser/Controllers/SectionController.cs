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
	public class SectionController : Controller
	{
		private OptimalEducationDbContext db = new OptimalEducationDbContext();
		private ApplicationDbContext dbIdentity = new ApplicationDbContext();
		public UserManager<ApplicationUser> UserManager { get; private set; }
		public SectionController()
		{
			UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbIdentity));
		}
		public SectionController(UserManager<ApplicationUser> userManager)
		{
			UserManager = userManager;
		}
		// GET: /EntrantUser/Section/
		public async Task<ActionResult> Index()
		{
			var entrantId = await GetEntrantId();
			var participationinSections = db.ParticipationInSections
				.Include(p => p.Entrants)
				.Include(p => p.Section)
				.Where(p => p.EntrantsId == entrantId);
			return View(await participationinSections.ToListAsync());
		}

		// GET: /EntrantUser/Section/Details/5
		public async Task<ActionResult> Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var entrantId = await GetEntrantId();
			ParticipationInSection participationinSection = await db.ParticipationInSections
				.Where(p => p.EntrantsId == entrantId)
                .SingleOrDefaultAsync(p => p.Id == id);
			if (participationinSection == null)
			{
				return HttpNotFound();
			}
			return View(participationinSection);
		}

		// GET: /EntrantUser/Section/Create
		public ActionResult Create()
		{
			ViewBag.SectionId = new SelectList(db.Sections, "Id", "Name");
			return View();
		}

		// POST: /EntrantUser/Section/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create([Bind(Include = "YearPeriod,SectionId")] ParticipationInSection participationinSection)
		{
			participationinSection.EntrantsId = await GetEntrantId();
			if (ModelState.IsValid)
			{
				db.ParticipationInSections.Add(participationinSection);
				await db.SaveChangesAsync();
				return RedirectToAction("Index");
			}

			ViewBag.SectionId = new SelectList(db.Sections, "Id", "Name", participationinSection.SectionId);
			return View(participationinSection);
		}

		// GET: /EntrantUser/Section/Edit/5
		public async Task<ActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var entrantId = await GetEntrantId();
			ParticipationInSection participationinSection = await db.ParticipationInSections
				.Where(p=>p.EntrantsId==entrantId)
                .SingleOrDefaultAsync(p => p.Id == id);
			if (participationinSection == null)
			{
				return HttpNotFound();
			}
			return View(participationinSection);
		}

		// POST: /EntrantUser/Section/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit([Bind(Include = "Id,YearPeriod")] ParticipationInSection participationinSection)
		{
			//var entrantId = await GetEntrantId();
			//participationinSection.EntrantId = entrantId;

			if (ModelState.IsValid)
			{
				var dbPartOlymp = await db.ParticipationInSections.FindAsync(participationinSection.Id);
				dbPartOlymp.YearPeriod = participationinSection.YearPeriod;
				await db.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			return View(participationinSection);
		}

		// GET: /EntrantUser/Section/Delete/5
		public async Task<ActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var entrantId = await GetEntrantId();
			ParticipationInSection participationinSection = await db.ParticipationInSections
				.Where(p => p.EntrantsId == entrantId)
				.SingleOrDefaultAsync(p => p.Id == id);
			if (participationinSection == null)
			{
				return HttpNotFound();
			}
			return View(participationinSection);
		}

		// POST: /EntrantUser/Section/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			var entrantId = await GetEntrantId();
			ParticipationInSection participationinSection = await db.ParticipationInSections
				.Where(p => p.EntrantsId == entrantId)
                .SingleOrDefaultAsync(p => p.Id == id);
			db.ParticipationInSections.Remove(participationinSection);
			await db.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		private async Task<int> GetEntrantId()
		{
			var currentUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var entrantClaim = currentUser.Claims.SingleOrDefault(p => p.ClaimType == MyClaimTypes.EntityUserId);
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
