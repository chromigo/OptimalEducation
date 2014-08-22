using Interfaces.CQRS;
using Microsoft.AspNet.Identity;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Interfaces.Logic.Characterizers;
using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis;
using OptimalEducation.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles=Role.Entrant)]
	public class RecomendationsController : Controller
	{
		private readonly IApplicationUserManager _userManager;
		private readonly IQueryBuilder _queryBuilder;
        private readonly IDistanceRecomendator<Entrant, EducationLine> _distanceRecomendator;
        private readonly IMulticriterialAnalysisRecomendator _multicriterialAnalysisRecomendator;
		public RecomendationsController(
            IApplicationUserManager userManager,
            IQueryBuilder queryBuilder,
            IDistanceRecomendator<Entrant,EducationLine> distanceRecomendator,
            IMulticriterialAnalysisRecomendator multicriterialAnalysisRecomendator)
		{
			_userManager = userManager;
			_queryBuilder=queryBuilder;
            _distanceRecomendator = distanceRecomendator;
            _multicriterialAnalysisRecomendator = multicriterialAnalysisRecomendator;
		}

		// GET: EntrantUser/Recomendations
		public async Task<ActionResult> Index()
		{
			var entrantId = await GetEntrantId();
            
			var entrant = await _queryBuilder
				.For<Task<Entrant>>()
                .With(new GetEntrantForCharacterizerCriterion() { EntrantId = entrantId });

			var educationLines = await _queryBuilder
                .For<Task<List<EducationLine>>>()
                .With(new GetEducationLinesForCharacterizerCriterion());

			//Рекомендации:
			//1. По методу сравнения расстояний мд характеристиками
            ViewBag.DistanceRecomendations = await _distanceRecomendator.GetRecomendation(entrant, educationLines);
			
			//2. По методу многокритериального анализа
            ViewBag.MulticriterialRecomendations =await _multicriterialAnalysisRecomendator.Calculate(entrant, educationLines);

			return View();
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