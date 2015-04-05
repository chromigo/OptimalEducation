using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Interfaces.CQRS;
using Microsoft.AspNet.Identity;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Helpers;
using OptimalEducation.Implementation.Logic.AHP;
using OptimalEducation.Interfaces.Logic.DistanceRecomendator;
using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis;
using OptimalEducation.Models;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
    [Authorize(Roles = Role.Entrant)]
    public class RecomendationsController : Controller
    {
        private readonly IDistanceRecomendator<Entrant, EducationLine> _distanceRecomendator;
        private readonly IInfoExtractor _infoExtractor;
        private readonly IMulticriterialAnalysisRecomendator _multicriterialAnalysisRecomendator;
        private readonly IQueryBuilder _queryBuilder;

        public RecomendationsController(
            IQueryBuilder queryBuilder,
            IDistanceRecomendator<Entrant, EducationLine> distanceRecomendator,
            IMulticriterialAnalysisRecomendator multicriterialAnalysisRecomendator,
            IInfoExtractor infoExtractor)
        {
            _queryBuilder = queryBuilder;
            _distanceRecomendator = distanceRecomendator;
            _multicriterialAnalysisRecomendator = multicriterialAnalysisRecomendator;
            _infoExtractor = infoExtractor;
        }

        // GET: EntrantUser/Recomendations
        public async Task<ActionResult> Index()
        {
            var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());

            var entrant = await _queryBuilder
                .For<Task<Entrant>>()
                .With(new GetEntrantForCharacterizerCriterion {EntrantId = entrantId});

            var educationLines = await _queryBuilder
                .For<Task<List<EducationLine>>>()
                .With(new GetEducationLinesForCharacterizerCriterion());

            //Рекомендации:
            //1. По методу сравнения расстояний мд характеристиками
            ViewBag.DistanceRecomendations = await _distanceRecomendator.GetRecomendation(entrant, educationLines);

            //2. По методу многокритериального анализа
            ViewBag.MulticriterialRecomendations =
                await _multicriterialAnalysisRecomendator.Calculate(entrant, educationLines);

            //3. По говнокоду AHP метод
            //TODO: выпилить этот говнокод вместе с проектом после публикации статьи(или переписать)
            await CalculateAhp(entrant, educationLines);

            return View();
        }

        private async Task CalculateAhp(Entrant entrant, List<EducationLine> educationLines)
        {
            var ahpUserAnalyzer = new AhpUser(entrant, educationLines);
            var orderedList = await ahpUserAnalyzer.CalculateAll();
            var tempAhpDict = new Dictionary<EducationLine, double>();
            foreach (var item in orderedList)
            {
                var edLine = educationLines.Find(p => p.Id == item.DatabaseId);
                tempAhpDict.Add(edLine, item.AbsolutePriority);
            }
            ViewBag.AHPRecomendations = tempAhpDict;
        }
    }
}