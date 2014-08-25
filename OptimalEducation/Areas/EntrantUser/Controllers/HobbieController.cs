using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OptimalEducation.DAL.Commands;
using OptimalEducation.DAL.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.Models;
using Interfaces.CQRS;
using OptimalEducation.DAL.ViewModels;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Helpers;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles = Role.Entrant)]
	public class HobbieController : Controller
	{
        private readonly IQueryBuilder _queryBuilder;
        private readonly ICommandBuilder _commandBuilder;
        private readonly IApplicationUserManager _userManager;
        private readonly IInfoExtractor _infoExtractor;

        public HobbieController(IQueryBuilder queryBuilder, ICommandBuilder commandBuilder, IApplicationUserManager userManager, IInfoExtractor infoExtractor)
        {
            _queryBuilder = queryBuilder;
            _commandBuilder = commandBuilder;
            _userManager = userManager;
            _infoExtractor = infoExtractor;
        }
		// GET: /EntrantUser/Hobbie/
		public async Task<ActionResult> Index()
		{
            var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
			var assignedHobbies = await _queryBuilder
				.For<Task<IEnumerable<AssignedHobbie>>>()
                .With(new GetAssignedHobbiesCriterion() { EntrantId = entrantId });
            return View(assignedHobbies);
		}

		//POST: /EntrantUser/UnitedStateExams/Index
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Index(string[] selectedHobbies)
		{
            var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());

            await _commandBuilder
                .ExecuteAsync<UpdateEntrantHobbieContext>(new UpdateEntrantHobbieContext() { EntrantId = entrantId, SelectedHobbies = selectedHobbies });

            return RedirectToAction("Index");
		}
	}
}
