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
using Interfaces.CQRS;
using OptimalEducation.DAL.Queries;
using OptimalEducation.DAL.Commands;
using OptimalEducation.Helpers;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles = Role.Entrant)]
	public class SectionController : Controller
	{
        private readonly IQueryBuilder _queryBuilder;
        private readonly ICommandBuilder _commandBuilder;
        private readonly IInfoExtractor _infoExtractor;

        public SectionController(IQueryBuilder queryBuilder, ICommandBuilder commandBuilder, IInfoExtractor infoExtractor)
        {
            _queryBuilder = queryBuilder;
            _commandBuilder = commandBuilder;
            _infoExtractor = infoExtractor;
        }
		// GET: /EntrantUser/Section/
		public async Task<ActionResult> Index()
		{
			var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());

			var participationinSections = await _queryBuilder
				.For<Task<IEnumerable<ParticipationInSection>>>()
				.With(new GetParticipationInSectionsOfEntrantCriterion{EntrantId= entrantId });

			return View(participationinSections);
		}

		// GET: /EntrantUser/Section/Details/5
		public async Task<ActionResult> Details(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());

            var participationinSection = await _queryBuilder
				.For<Task<ParticipationInSection>>()
				.With(new GetCurrentParticipationInSectionsOfEntrantCriterion() { EntrantId=entrantId, Id=id.Value });

			if (participationinSection == null)
			{
				return HttpNotFound();
			}
			return View(participationinSection);
		}

		// GET: /EntrantUser/Section/Create
		public async Task<ActionResult> Create()
		{
			var sections = await _queryBuilder
				.For<Task<IEnumerable<Section>>>()
				.With(new GetAllSectionsCriterion());

            ViewBag.SectionId = new SelectList(sections, "Id", "Name");
			return View();
		}

		// POST: /EntrantUser/Section/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create([Bind(Include = "YearPeriod,SectionId")] ParticipationInSection participationInSection)
		{
            participationInSection.EntrantsId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
			if (ModelState.IsValid)
			{
                await _commandBuilder
                    .ExecuteAsync<AddParticipationInSectionContext>(new AddParticipationInSectionContext() { ParticipationInSection = participationInSection });

				return RedirectToAction("Index");
			}
             
			var sections = await _queryBuilder
				.For<Task<IEnumerable<Section>>>()
				.With(new GetAllSectionsCriterion());

            ViewBag.SectionId = new SelectList(sections, "Id", "Name", participationInSection.SectionId);
			return View(participationInSection);
		}

		// GET: /EntrantUser/Section/Edit/5
		public async Task<ActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
            var participationinSection = await _queryBuilder
                .For<Task<ParticipationInSection>>()
                .With(new GetCurrentParticipationInSectionsOfEntrantCriterion() { EntrantId = entrantId, Id = id.Value });
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
		public async Task<ActionResult> Edit([Bind(Include = "Id,YearPeriod")] ParticipationInSection participationInSection)
		{
			if (ModelState.IsValid)
			{
                await _commandBuilder
                    .ExecuteAsync<UpdateParticipationInSectionResultContext>(new UpdateParticipationInSectionResultContext() { ParticipationInSection = participationInSection });

				return RedirectToAction("Index");
			}
			return View(participationInSection);
		}

		// GET: /EntrantUser/Section/Delete/5
		public async Task<ActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
            var participationinSection = await _queryBuilder
                .For<Task<ParticipationInSection>>()
                .With(new GetCurrentParticipationInSectionsOfEntrantCriterion() { EntrantId = entrantId, Id = id.Value });
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
			var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());

            await _commandBuilder
                .ExecuteAsync<RemoveParticipationInSectionContext>(new RemoveParticipationInSectionContext() { EntrantId = entrantId, ParticipationInSectionId = id });

			return RedirectToAction("Index");
		}
	}
}
