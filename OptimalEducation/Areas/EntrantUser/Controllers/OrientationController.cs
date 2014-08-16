using CQRS;
using Microsoft.AspNet.Identity;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Logic.Characterizer;
using OptimalEducation.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles=Role.Entrant)]
	public class OrientationController : Controller
	{
	    private readonly IApplicationUserManager _userManager;
        private readonly IQueryBuilder _queryBuilder;
        private readonly ICharacterizer<Entrant> _entrantCharacterizer;
        public OrientationController(IApplicationUserManager userManager,IQueryBuilder queryBuilder, ICharacterizer<Entrant> entrantCharacterizer)
		{
		    _userManager = userManager;
            _queryBuilder = queryBuilder;
            _entrantCharacterizer = entrantCharacterizer;
		}

	    // GET: /EntrantUser/Orientation/
		public async Task<ActionResult> Index()
		{
			var entrantId = await GetEntrantId();
            var entrant = await _queryBuilder.For<Task<Entrant>>().With(new GetEntrantForCharacterizerCriterion() { EntrantId = entrantId });

            //Предпочтения пользователя по предметам и пр.
            var entrantCharacteristics = _entrantCharacterizer.Calculate(entrant);//add true for complicated method
            ViewBag.Preferences = entrantCharacteristics;

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