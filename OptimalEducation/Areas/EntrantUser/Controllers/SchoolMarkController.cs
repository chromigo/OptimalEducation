using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OptimalEducation.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.DAL.Models;
using Interfaces.CQRS;
using OptimalEducation.DAL.Queries;
using OptimalEducation.DAL.Commands;
using OptimalEducation.Helpers;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles = Role.Entrant)]
	public class SchoolMarkController : Controller
	{
		private readonly IQueryBuilder _queryBuilder;
		private readonly ICommandBuilder _commandBuilder;
        private readonly IInfoExtractor _infoExtractor;

        public SchoolMarkController(IQueryBuilder queryBuilder, ICommandBuilder commandBuilder, IInfoExtractor infoExtractor)
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
				.With(new GetSchoolMarksOfEntrantCriterion() { EntrantId = entrantId });

			return View(schoolMarks);
		}

        // POST: /EntrantUser/SchoolMark/Index
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Index([Bind(Include = "SchoolDisciplineId,Result")] IEnumerable<SchoolMark> schoolMark)
		{
			if (ModelState.IsValid)
			{
				var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());

				await _commandBuilder
                        .ExecuteAsync<UpdateSchoolMarkOfEntrantContext>(new UpdateSchoolMarkOfEntrantContext() { EntrantId = entrantId, SchoolMark = schoolMark });

				return RedirectToAction("Index");
			}

			return View(schoolMark);
		}
	}
}
