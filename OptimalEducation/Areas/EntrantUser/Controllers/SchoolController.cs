using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Interfaces.CQRS;
using Microsoft.AspNet.Identity;
using OptimalEducation.DAL.Commands.ParticipationInSchool;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;
using OptimalEducation.DAL.Queries.ParticipationInSchool;
using OptimalEducation.Helpers;
using OptimalEducation.Models;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
    [Authorize(Roles = Role.Entrant)]
    public class SchoolController : Controller
    {
        private readonly ICommandBuilder _commandBuilder;
        private readonly IInfoExtractor _infoExtractor;
        private readonly IQueryBuilder _queryBuilder;

        public SchoolController(IQueryBuilder queryBuilder, ICommandBuilder commandBuilder, IInfoExtractor infoExtractor)
        {
            _queryBuilder = queryBuilder;
            _commandBuilder = commandBuilder;
            _infoExtractor = infoExtractor;
        }

        // GET: /EntrantUser/School/
        public async Task<ActionResult> Index()
        {
            var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
            var participationinSchools = await _queryBuilder
                .For<Task<IEnumerable<ParticipationInSchool>>>()
                .With(new GetParticipationInSchoolOfEntrantCriterion {EntrantId = entrantId});

            return View(participationinSchools);
        }

        // GET: /EntrantUser/School/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
            var participationinSchool = await _queryBuilder
                .For<Task<ParticipationInSchool>>()
                .With(new GetCurrentParticipationInSchoolOfEntrantCriterion {EntrantId = entrantId, Id = id.Value});

            if (participationinSchool == null)
            {
                return HttpNotFound();
            }
            return View(participationinSchool);
        }

        // GET: /EntrantUser/School/Create
        public async Task<ActionResult> Create()
        {
            var allSchools = await _queryBuilder
                .For<Task<IEnumerable<School>>>()
                .With(new GetAllSchoolsCriterion());
            ViewBag.SchoolId = new SelectList(allSchools, "Id", "Name");
            return View();
        }

        // POST: /EntrantUser/School/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "YearPeriod,SchoolId")] ParticipationInSchool participationInSchool)
        {
            participationInSchool.EntrantsId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                await _commandBuilder
                    .ExecuteAsync(new AddParticipationInSchoolContext {ParticipationInSchool = participationInSchool});
                return RedirectToAction("Index");
            }
            var allSchools = await _queryBuilder
                .For<Task<IEnumerable<School>>>()
                .With(new GetAllSchoolsCriterion());
            ViewBag.SchoolId = new SelectList(allSchools, "Id", "Name", participationInSchool.SchoolId);
            return View(participationInSchool);
        }

        // GET: /EntrantUser/School/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
            var participationinSchool = await _queryBuilder
                .For<Task<ParticipationInSchool>>()
                .With(new GetCurrentParticipationInSchoolOfEntrantCriterion {EntrantId = entrantId, Id = id.Value});
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
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,YearPeriod")] ParticipationInSchool participationInSchool)
        {
            if (ModelState.IsValid)
            {
                await _commandBuilder
                    .ExecuteAsync(new UpdateParticipationInSchoolContext {ParticipationInSchool = participationInSchool});

                return RedirectToAction("Index");
            }
            return View(participationInSchool);
        }

        // GET: /EntrantUser/School/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
            var participationinSchool = await _queryBuilder
                .For<Task<ParticipationInSchool>>()
                .With(new GetCurrentParticipationInSchoolOfEntrantCriterion {EntrantId = entrantId, Id = id.Value});
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
            var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());

            await _commandBuilder
                .ExecuteAsync(new RemoveParticipationInShoolContext {EntrantId = entrantId, Id = id});

            return RedirectToAction("Index");
        }
    }
}