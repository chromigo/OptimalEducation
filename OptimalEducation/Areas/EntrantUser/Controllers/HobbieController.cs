using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Interfaces.CQRS;
using Microsoft.AspNet.Identity;
using OptimalEducation.DAL.Commands;
using OptimalEducation.DAL.Queries;
using OptimalEducation.DAL.ViewModels;
using OptimalEducation.Helpers;
using OptimalEducation.Models;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
    [Authorize(Roles = Role.Entrant)]
    public class HobbieController : Controller
    {
        private readonly ICommandBuilder _commandBuilder;
        private readonly IInfoExtractor _infoExtractor;
        private readonly IQueryBuilder _queryBuilder;

        public HobbieController(IQueryBuilder queryBuilder, ICommandBuilder commandBuilder, IInfoExtractor infoExtractor)
        {
            _queryBuilder = queryBuilder;
            _commandBuilder = commandBuilder;
            _infoExtractor = infoExtractor;
        }

        // GET: /EntrantUser/Hobbie/
        public async Task<ActionResult> Index()
        {
            var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
            var assignedHobbies = await _queryBuilder
                .For<Task<IEnumerable<AssignedHobbie>>>()
                .With(new GetAssignedHobbiesCriterion {EntrantId = entrantId});
            return View(assignedHobbies);
        }

        //POST: /EntrantUser/UnitedStateExams/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string[] selectedHobbies)
        {
            var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());

            await _commandBuilder
                .ExecuteAsync(new UpdateEntrantHobbieContext {EntrantId = entrantId, SelectedHobbies = selectedHobbies});

            return RedirectToAction("Index");
        }
    }
}