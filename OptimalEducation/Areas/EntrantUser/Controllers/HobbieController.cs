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
		private readonly OptimalEducationDbContext _dbContext;
	    private readonly UserManager<ApplicationUser> _userManager;

        public HobbieController(OptimalEducationDbContext dbContext, UserManager<ApplicationUser> userManager)
		{
            _dbContext = dbContext;
            _userManager = userManager;
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

				var userHobbieIdsQuery = (from entrant in _dbContext.Entrants.Include(p=>p.Hobbies)
						where entrant.Id==entrantId
						from hobbie in entrant.Hobbies
						select hobbie.Id);

				var userHobbieIds = new HashSet<int>(userHobbieIdsQuery);
				var allHobbies = await _dbContext.Hobbies.ToListAsync<Hobbie>();

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
			var currentEntrant =  await _dbContext.Entrants.SingleAsync(p => p.Id == entrantId);
			var allHobbies = await _dbContext.Hobbies.ToListAsync<Hobbie>();
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
			_dbContext.Entry(currentEntrant).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
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
