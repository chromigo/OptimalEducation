using System.Threading.Tasks;
using System.Web.Mvc;
using Interfaces.CQRS;
using Microsoft.AspNet.Identity;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Helpers;
using OptimalEducation.Interfaces.Logic.Characterizers;
using OptimalEducation.Models;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
    [Authorize(Roles = Role.Entrant)]
    public class OrientationController : Controller
    {
        private readonly ICharacterizer<Entrant> _entrantCharacterizer;
        private readonly IInfoExtractor _infoExtractor;
        private readonly IQueryBuilder _queryBuilder;

        public OrientationController(IQueryBuilder queryBuilder, ICharacterizer<Entrant> entrantCharacterizer,
            IInfoExtractor infoExtractor)
        {
            _queryBuilder = queryBuilder;
            _entrantCharacterizer = entrantCharacterizer;
            _infoExtractor = infoExtractor;
        }

        // GET: /EntrantUser/Orientation/
        public async Task<ActionResult> Index()
        {
            var entrantId = await _infoExtractor.ExtractEntrantId(User.Identity.GetUserId());
            var entrant =
                await
                    _queryBuilder.For<Task<Entrant>>()
                        .With(new GetEntrantForCharacterizerCriterion {EntrantId = entrantId});

            //Предпочтения пользователя по предметам и пр.
            var entrantCharacteristics = await _entrantCharacterizer.Calculate(entrant);
                //add true for complicated method
            ViewBag.Preferences = entrantCharacteristics;

            return View();
        }
    }
}