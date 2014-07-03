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
        private readonly OptimalEducationDbContext _dbContext;
        private readonly IApplicationUserManager _userManager;

        public SchoolController(OptimalEducationDbContext dbContext, IApplicationUserManager userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
		// GET: /EntrantUser/School/
		public async Task<ActionResult> Index()
		{
			var entrantId = await GetEntrantId();
			var participationinSchools = _dbContext.ParticipationInSchools
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
			ParticipationInSchool participationinSchool = await _dbContext.ParticipationInSchools
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
			ViewBag.SchoolId = new SelectList(_dbContext.Schools, "Id", "Name");
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
				_dbContext.ParticipationInSchools.Add(participationinSchool);
				await _dbContext.SaveChangesAsync();
				return RedirectToAction("Index");
			}

			ViewBag.SchoolId = new SelectList(_dbContext.Schools, "Id", "Name", participationinSchool.SchoolId);
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
			ParticipationInSchool participationinSchool = await _dbContext.ParticipationInSchools
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
				var dbPartOlymp = await _dbContext.ParticipationInSchools.FindAsync(participationinSchool.Id);
                dbPartOlymp.YearPeriod = participationinSchool.YearPeriod;
				await _dbContext.SaveChangesAsync();
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
			ParticipationInSchool participationinSchool = await _dbContext.ParticipationInSchools
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
			ParticipationInSchool participationinSchool = await _dbContext.ParticipationInSchools
				.Where(p => p.EntrantsId == entrantId)
				.FirstOrDefaultAsync(p => p.Id == id);
			_dbContext.ParticipationInSchools.Remove(participationinSchool);
			await _dbContext.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		private async Task<int> GetEntrantId()
		{
			var currentUser = await _userManager.FindByIdAsync(User.Identity.GetUserId());
			var entrantClaim = currentUser.Claims.FirstOrDefault(p => p.ClaimType == MyClaimTypes.EntityUserId);
			var entrantId = int.Parse(entrantClaim.ClaimValue);
			return entrantId;
		}
	}
}
