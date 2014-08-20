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
using OptimalEducation.Logic.Characterizers;
using OptimalEducation.DAL.Models;
using CQRS;
using OptimalEducation.DAL.Queries;
using OptimalEducation.DAL.Commands;


namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles=Role.Entrant)]
	public class UnitedStateExamsController : Controller
	{
        private readonly IApplicationUserManager _userManager;
        private readonly IQueryBuilder _queryBuilder;
        private readonly ICommandBuilder _commandBuilder;

        public UnitedStateExamsController(IApplicationUserManager userManager, IQueryBuilder queryBuilder, ICommandBuilder commandBuilder)
        {
            _userManager = userManager;
            _queryBuilder = queryBuilder;
            _commandBuilder = commandBuilder;
        }
		// GET: /EntrantUser/UnitedStateExams/
		public async Task<ActionResult> Index()
		{
            var entrantId = await GetEntrantId();
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
                var entrantId = await GetEntrantId();

                await _commandBuilder
                        .ExecuteAsync<UpdateUnitedStateExamOfEntrantContext>(new UpdateUnitedStateExamOfEntrantContext() { EntrantId = entrantId, UnitedStateExams = unitedStateExams });
                return RedirectToAction("Index");
			}

			return View(unitedStateExams);
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
