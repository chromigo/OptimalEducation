﻿using Interfaces.CQRS;
using Microsoft.AspNet.Identity;
using OptimalEducation.DAL.Commands;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Helpers;
using OptimalEducation.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SimpleInjector.Diagnostics;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles = Role.Entrant)]
	public class OlympiadController : Controller
	{
        private readonly IQueryBuilder _queryBuilder;
	    private readonly ICommandBuilder _commandBuilder;
        private readonly IInfoExtractor _infoExtractor;

        public OlympiadController(IQueryBuilder queryBuilder, ICommandBuilder commandBuilder, IInfoExtractor infoExtractor)
	    {
            _queryBuilder = queryBuilder;
            _commandBuilder = commandBuilder;
            _infoExtractor = infoExtractor;
	    }

	    // GET: /EntrantUser/Olympiad/

	    public async Task<ActionResult> Index()
		{
			var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
		    var participationinolympiads = await _queryBuilder
                                                    .For<Task<IEnumerable<ParticipationInOlympiad>>>()
		                                            .With(new GetAllParticipationInOlympiadCriterion() {EntrantId = entrantId});
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
                var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
                var participationinolympiads = await _queryBuilder.For<Task<ParticipationInOlympiad>>()
                    .With(new GetCurrentParticipationInOlympiadCriterion() { EntrantId = entrantId,ParticipationInOlympiadId  = poId});

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
                                        .With(new GetAllOlympiadsCriterion());
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
			participationinolympiad.EntrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
			if (ModelState.IsValid)
			{
                await _commandBuilder
                    .ExecuteAsync(new AddParticipationInOlympiadContext(){ParticipationInOlympiad = participationinolympiad});

				return RedirectToAction("Index");
			}

            var olympiads = await _queryBuilder
                            .For<Task<IEnumerable<Olympiad>>>()
                            .With(new GetAllOlympiadsCriterion());
            ViewBag.OlympiadId = new SelectList(olympiads, "Id", "Name", participationinolympiad.OlympiadId);

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
		    var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
		    var participationinOlympiads = await _queryBuilder.For<Task<ParticipationInOlympiad>>()
		        .With(new GetCurrentParticipationInOlympiadCriterion() { EntrantId = entrantId, ParticipationInOlympiadId = poId });

		    if (participationinOlympiads == null)
		    {
		    }
		    return HttpNotFound();
		    return View(participationinOlympiads);
		}

		// POST: /EntrantUser/Olympiad/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit([Bind(Include="Id,Result")] ParticipationInOlympiad participationinolympiad)
		{
			//var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
			//participationinolympiad.EntrantId = entrantId;

			if (ModelState.IsValid)
			{
                await _commandBuilder
                        .ExecuteAsync(new UpdateParticipationInOlympiadResultContext() { ParticipationInOlympiadId = participationinolympiad.Id, UpdateResult = participationinolympiad.Result });

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
            var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
            var participationinolympiad = await _queryBuilder
                .For<Task<ParticipationInOlympiad>>()
                .With(new GetCurrentParticipationInOlympiadCriterion() { EntrantId = entrantId, ParticipationInOlympiadId = participationInOlympiadId });

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
			var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());

            await _commandBuilder
                .ExecuteAsync(new RemoveParticipationInOlympiadContext(){EntrantId = entrantId,ParticipationInOlympiadId = id});

			return RedirectToAction("Index");
		}
	}
}
