using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CQRS;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles = Role.Entrant)]
	public class OlympiadController : Controller
	{
        private readonly IQueryBuilder _queryBuilder;
	    private readonly IApplicationUserManager _userManager;

        public OlympiadController(IQueryBuilder queryBuilder, IApplicationUserManager userManager)
	    {
            _queryBuilder = queryBuilder;
	        _userManager = userManager;
	    }

	    // GET: /EntrantUser/Olympiad/
		public async Task<ActionResult> Index()
		{
			var entrantId = await GetEntrantId();
		    var participationinolympiads = await _queryBuilder
                                                    .For<Task<IEnumerable<ParticipationInOlympiad>>>()
		                                            .With(new GetAllParticipationInOlympiad() {EntrantId = entrantId});
            return View(participationinolympiads);
		}

		// GET: /EntrantUser/Olympiad/Details/5
		public async Task<ActionResult> Details(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			else
			{
			    var poId = id.Value;
                var entrantId = await GetEntrantId();
                var participationinolympiads = await _queryBuilder.For<Task<ParticipationInOlympiad>>()
                    .With(new GetCurrentParticipationInOlympiad() { EntrantId = entrantId,ParticipationInOlympiadId  = poId});

                if (participationinolympiads == null)
                {
                    return HttpNotFound();
                }
                return View(participationinolympiads);
			}
		}

		// GET: /EntrantUser/Olympiad/Create
		public async Task<ActionResult> Create()
		{
		    var olympiads = await _queryBuilder
                                        .For<Task<IEnumerable<Olympiad>>>()
                                        .With(new GetAllOlympiads());
			ViewBag.OlympiadId = new SelectList(olympiads, "Id", "Name");
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
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
            var poId = id.Value;
            var entrantId = await GetEntrantId();
            var participationinOlympiads = await _queryBuilder.For<Task<ParticipationInOlympiad>>()
                .With(new GetCurrentParticipationInOlympiad() { EntrantId = entrantId, ParticipationInOlympiadId = poId });

            if (participationinOlympiads == null)
			{
				return HttpNotFound();
			}
            return View(participationinOlympiads);
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
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
		    var participationInOlympiadId = id.Value;
            var entrantId = await GetEntrantId();
            var participationinolympiad = await _queryBuilder
                .For<Task<ParticipationInOlympiad>>()
                .With(new GetCurrentParticipationInOlympiad() { EntrantId = entrantId, ParticipationInOlympiadId = participationInOlympiadId });

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
			var participationinolympiad = await _dbContext.ParticipationInOlympiads
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
