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
using OptimalEducation.Areas.EntrantUser.Models.ViewModels;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	public class HobbieController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();
		public UserManager<ApplicationUser> UserManager { get; private set; }

		public HobbieController()
		{
			UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
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
			var currentUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			var allHobbies = await db.Hobbies.ToListAsync<Hobbie>();
			var userHobbiesId = new HashSet<int>(currentUser.Entrant.Hobbies.Select(c => c.Id));

			var viewModel = new List<AssignedHobbie>();
			foreach (var hobbie in allHobbies)
			{
				viewModel.Add(new AssignedHobbie
				{
					Id = hobbie.Id,
					Name = hobbie.Name,
					IsAssigned = userHobbiesId.Contains(hobbie.Id)
				});
			}
			return viewModel;
		}

		//POST: /EntrantUser/UnitedStateExams/Index
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Index(string[] selectedHobbies)
		{
			var currentEntrant = (await UserManager.FindByIdAsync(User.Identity.GetUserId())).Entrant;
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

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
