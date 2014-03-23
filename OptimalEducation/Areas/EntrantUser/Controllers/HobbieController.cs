using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OptimalEducation.DAL.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.Areas.EntrantUser.Models.ViewModels;
using OptimalEducation.Models;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles = Role.Entrant)]
	public class HobbieController : Controller
	{
		private OptimalEducationDbContext db = new OptimalEducationDbContext();
		private ApplicationDbContext dbIdentity = new ApplicationDbContext();

		public UserManager<ApplicationUser> UserManager { get; private set; }

		public HobbieController()
		{
			UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbIdentity));
		}
		public HobbieController(UserManager<ApplicationUser> userManager)
		{
			UserManager = userManager;
		}
		// GET: /EntrantUser/Hobbie/
		public async Task<ActionResult> Index()
		{
			var hobbieList = await GetUserHobbieAsync();
			return View(hobbieList);
		}
		private async Task<List<AssignedHobbie>> GetUserHobbieAsync()
		{
			try 
			{
				var entrantId = await GetEntrantId();

				var userHobbieIdsQuery = (from entrant in db.Entrants.Include(p=>p.Hobbies)
						where entrant.Id==entrantId
						from hobbie in entrant.Hobbies
						select hobbie.Id);

				var userHobbieIds = new HashSet<int>(userHobbieIdsQuery);
				var allHobbies = await db.Hobbies.ToListAsync<Hobbie>();

				var viewModel = new List<AssignedHobbie>();
				foreach (var hobbie in allHobbies)
				{
					viewModel.Add(new AssignedHobbie
					{
						Id = hobbie.Id,
						Name = hobbie.Name,
						IsAssigned = userHobbieIds.Contains(hobbie.Id)
					});
				}
				return viewModel;
			}
			catch (Exception)
			{
				throw;
			}
		}

		//POST: /EntrantUser/UnitedStateExams/Index
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Index(string[] selectedHobbies)
		{
			var entrantId = await GetEntrantId();
			var currentEntrant =  await db.Entrants.SingleAsync(p => p.Id == entrantId);
			var allHobbies = await db.Hobbies.ToListAsync<Hobbie>();
			if (selectedHobbies == null)
			{
				foreach (var hobbie in allHobbies)
				{
					currentEntrant.Hobbies.Remove(hobbie);
				}
			}
			else
			{
				var selectedHobbiesList = new List<int>();
				foreach (var hobbie in selectedHobbies)
				{
					selectedHobbiesList.Add(int.Parse(hobbie));
				}

				var lastUserHobbieIds = currentEntrant.Hobbies.Select(h=>h.Id);
				foreach (var hobbie in allHobbies)
				{
					if (selectedHobbiesList.Contains(hobbie.Id))
					{
						//Если не было - добавляем
						if (!lastUserHobbieIds.Contains(hobbie.Id))
							currentEntrant.Hobbies.Add(hobbie);
					}
					else//не выбранное хобби
					{
						//Если было - удаляем
						if (lastUserHobbieIds.Contains(hobbie.Id))
							currentEntrant.Hobbies.Remove(hobbie);
					}
				}
			}
			db.Entry(currentEntrant).State = EntityState.Modified;
			await db.SaveChangesAsync();
			return RedirectToAction("Index");
		}
        private async Task<int> GetEntrantId()
        {
            var currentUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var entrantClaim = currentUser.Claims.FirstOrDefault(p => p.ClaimType == MyClaimTypes.EntityUserId);
            var entrantId = int.Parse(entrantClaim.ClaimValue);
            return entrantId;
        }
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
				dbIdentity.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
