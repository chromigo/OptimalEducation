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

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles = Role.Entrant)]
	public class HobbieController : Controller
	{
        private readonly IQueryBuilder _queryBuilder;
        private readonly ICommandBuilder _commandBuilder;
        private readonly IApplicationUserManager _userManager;

        public HobbieController(IQueryBuilder queryBuilder, ICommandBuilder commandBuilder, IApplicationUserManager userManager)
        {
            _queryBuilder = queryBuilder;
            _commandBuilder = commandBuilder;
            _userManager = userManager;
        }
		// GET: /EntrantUser/Hobbie/
		public async Task<ActionResult> Index()
		{
            var entrantId = await GetEntrantId();
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
			var entrantId = await GetEntrantId();

            await _commandBuilder
                .ExecuteAsync<UpdateEntrantHobbieContext>(new UpdateEntrantHobbieContext() { EntrantId = entrantId, SelectedHobbies = selectedHobbies });

            return RedirectToAction("Index");
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
