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

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles = Role.Entrant)]
	public class SchoolMarkController : Controller
	{
		private readonly IApplicationUserManager _userManager;
		private readonly IQueryBuilder _queryBuilder;
		private readonly ICommandBuilder _commandBuilder;
		public SchoolMarkController(IApplicationUserManager userManager, IQueryBuilder queryBuilder, ICommandBuilder commandBuilder)
		{
			_userManager = userManager;
			_queryBuilder = queryBuilder;
			_commandBuilder = commandBuilder;
		}
		// GET: /EntrantUser/SchoolMark/
		public async Task<ActionResult> Index()
		{
			var entrantId = await GetEntrantId();

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
				var entrantId = await GetEntrantId();

				await _commandBuilder
                        .ExecuteAsync<UpdateSchoolMarkOfEntrantContext>(new UpdateSchoolMarkOfEntrantContext() { EntrantId = entrantId, SchoolMark = schoolMark });

				return RedirectToAction("Index");
			}

			return View(schoolMark);
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
