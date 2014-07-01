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
	public class OlympiadController : Controller
	{
		private readonly OptimalEducationDbContext _dbContext;
	    private readonly UserManager<ApplicationUser> _userManager;

        public OlympiadController(OptimalEducationDbContext dbContext, UserManager<ApplicationUser> userManager)
	    {
            _dbContext = dbContext;
	        _userManager = userManager;
	    }

	    // GET: /EntrantUser/Olympiad/
		public async Task<ActionResult> Index()
		{
			var entrantId = await GetEntrantId();
			var participationinolympiads = _dbContext.ParticipationInOlympiads
				.Include(p => p.Entrant)
				.Include(p => p.Olympiad)
				.Where(p => p.EntrantId == entrantId);
			return View(await participationinolympiads.ToListAsync());
		}

		// GET: /EntrantUser/Olympiad/Details/5
		public async Task<ActionResult> Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var entrantId = await GetEntrantId();
			ParticipationInOlympiad participationinolympiad = await _dbContext.ParticipationInOlympiads
				.Where(p => p.EntrantId == entrantId)
				.FirstOrDefaultAsync(p => p.Id == id);
			if (participationinolympiad == null)
			{
				return HttpNotFound();
			}
			return View(participationinolympiad);
		}

		// GET: /EntrantUser/Olympiad/Create
		public ActionResult Create()
		{
			ViewBag.OlympiadId = new SelectList(_dbContext.Olympiads, "Id", "Name");
			return View();
		}

		// POST: /EntrantUser/Olympiad/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create([Bind(Include="Result,OlympiadId")] ParticipationInOlympiad participationinolympiad)
		{
			participationinolympiad.EntrantId = await GetEntrantId();
			if (ModelState.IsValid)
			{
				_dbContext.ParticipationInOlympiads.Add(participationinolympiad);
				await _dbContext.SaveChangesAsync();
				return RedirectToAction("Index");
			}

			ViewBag.OlympiadId = new SelectList(_dbContext.Olympiads, "Id", "Name", participationinolympiad.OlympiadId);
			return View(participationinolympiad);
		}

		// GET: /EntrantUser/Olympiad/Edit/5
		public async Task<ActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var entrantId = await GetEntrantId();
			ParticipationInOlympiad participationinolympiad = await _dbContext.ParticipationInOlympiads
				.Where(p=>p.EntrantId==entrantId)
				.FirstOrDefaultAsync(p=>p.Id==id);
			if (participationinolympiad == null)
			{
				return HttpNotFound();
			}
			return View(participationinolympiad);
		}

		// POST: /EntrantUser/Olympiad/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit([Bind(Include="Id,Result")] ParticipationInOlympiad participationinolympiad)
		{
			//var entrantId = await GetEntrantId();
			//participationinolympiad.EntrantId = entrantId;

			if (ModelState.IsValid)
			{
				var dbPartOlymp = await _dbContext.ParticipationInOlympiads.FindAsync(participationinolympiad.Id);
				dbPartOlymp.Result = participationinolympiad.Result;
				await _dbContext.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			return View(participationinolympiad);
		}

		// GET: /EntrantUser/Olympiad/Delete/5
		public async Task<ActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var entrantId = await GetEntrantId();
			ParticipationInOlympiad participationinolympiad = await _dbContext.ParticipationInOlympiads
				.Where(p => p.EntrantId == entrantId)
				.FirstOrDefaultAsync(p => p.Id == id);
			if (participationinolympiad == null)
			{
				return HttpNotFound();
			}
			return View(participationinolympiad);
		}

		// POST: /EntrantUser/Olympiad/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			var entrantId = await GetEntrantId();
			ParticipationInOlympiad participationinolympiad = await _dbContext.ParticipationInOlympiads
				.Where(p => p.EntrantId == entrantId)
				.FirstOrDefaultAsync(p => p.Id == id);
			_dbContext.ParticipationInOlympiads.Remove(participationinolympiad);
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
