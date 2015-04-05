using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Interfaces.CQRS;
using Microsoft.AspNet.Identity;
using OptimalEducation.DAL.Commands;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Helpers;
using OptimalEducation.Models;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
    [Authorize(Roles = Role.Entrant)]
    public class UnitedStateExamsController : Controller
    {
        private readonly ICommandBuilder _commandBuilder;
        private readonly IInfoExtractor _infoExtractor;
        private readonly IQueryBuilder _queryBuilder;

        public UnitedStateExamsController(IQueryBuilder queryBuilder, ICommandBuilder commandBuilder,
            IInfoExtractor infoExtractor)
        {
            _queryBuilder = queryBuilder;
            _commandBuilder = commandBuilder;
            _infoExtractor = infoExtractor;
        }

        // GET: /EntrantUser/UnitedStateExams/
        public async Task<ActionResult> Index()
        {
            var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
            var usExam = await _queryBuilder
                .For<Task<IEnumerable<UnitedStateExam>>>()
                .With(new GetUnitedStateExamsOfEntrantCriterion {EntrantId = entrantId});
            return View(usExam);
        }

        // POST: /EntrantUser/UnitedStateExams/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(
            [Bind(Include = "ExamDisciplineId,Result")] IEnumerable<UnitedStateExam> unitedStateExams)
        {
            if (ModelState.IsValid)
            {
                var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());

                await _commandBuilder
                    .ExecuteAsync(new UpdateUnitedStateExamOfEntrantContext
                    {
                        EntrantId = entrantId,
                        UnitedStateExams = unitedStateExams
                    });
                return RedirectToAction("Index");
            }

            return View(unitedStateExams);
        }
    }
}