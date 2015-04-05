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
    public class SchoolMarkController : Controller
    {
        private readonly ICommandBuilder _commandBuilder;
        private readonly IInfoExtractor _infoExtractor;
        private readonly IQueryBuilder _queryBuilder;

        public SchoolMarkController(IQueryBuilder queryBuilder, ICommandBuilder commandBuilder,
            IInfoExtractor infoExtractor)
        {
            _queryBuilder = queryBuilder;
            _commandBuilder = commandBuilder;
            _infoExtractor = infoExtractor;
        }

        // GET: /EntrantUser/SchoolMark/
        public async Task<ActionResult> Index()
        {
            var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());

            var schoolMarks = await _queryBuilder
                .For<Task<IEnumerable<SchoolMark>>>()
                .With(new GetSchoolMarksOfEntrantCriterion {EntrantId = entrantId});

            return View(schoolMarks);
        }

        // POST: /EntrantUser/SchoolMark/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(
            [Bind(Include = "SchoolDisciplineId,Result")] IEnumerable<SchoolMark> schoolMark)
        {
            if (ModelState.IsValid)
            {
                var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());

                await _commandBuilder
                    .ExecuteAsync(new UpdateSchoolMarkOfEntrantContext {EntrantId = entrantId, SchoolMark = schoolMark});

                return RedirectToAction("Index");
            }

            return View(schoolMark);
        }
    }
}