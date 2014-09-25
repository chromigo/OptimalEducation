using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using OptimalEducation.Models;
using Microsoft.AspNet.Identity;
using OptimalEducation.DAL.Models;
using Interfaces.CQRS;
using OptimalEducation.DAL.Queries;
using OptimalEducation.DAL.Commands;
using OptimalEducation.Helpers;


namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles=Role.Entrant)]
	public class UnitedStateExamsController : Controller
	{
        private readonly IQueryBuilder _queryBuilder;
        private readonly ICommandBuilder _commandBuilder;
        private readonly IInfoExtractor _infoExtractor;

        public UnitedStateExamsController(IQueryBuilder queryBuilder, ICommandBuilder commandBuilder, IInfoExtractor infoExtractor)
        {
            _queryBuilder = queryBuilder;
            _commandBuilder = commandBuilder;
            _infoExtractor = infoExtractor;
        }
		// GET: /EntrantUser/UnitedStateExams/
		public async Task<ActionResult> Index()
		{
            var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
            var USExam = await _queryBuilder
                    .For<Task<IEnumerable<UnitedStateExam>>>()
                    .With(new GetUnitedStateExamsOfEntrantCriterion() { EntrantId = entrantId });
            return View(USExam);
		}

		// POST: /EntrantUser/UnitedStateExams/Index
		[HttpPost]
		[ValidateAntiForgeryToken]
        public async Task<ActionResult> Index([Bind(Include = "ExamDisciplineId,Result")] IEnumerable<UnitedStateExam> unitedStateExams)
		{
			if (ModelState.IsValid)
			{
                var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());

                await _commandBuilder
                        .ExecuteAsync<UpdateUnitedStateExamOfEntrantContext>(new UpdateUnitedStateExamOfEntrantContext() { EntrantId = entrantId, UnitedStateExams = unitedStateExams });
                return RedirectToAction("Index");
			}

			return View(unitedStateExams);
		}
	}
}
