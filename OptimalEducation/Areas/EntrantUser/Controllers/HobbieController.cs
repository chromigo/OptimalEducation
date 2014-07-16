﻿using System;
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
using OptimalEducation.Areas.EntrantUser.Models.ViewModels;
using OptimalEducation.Models;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles = Role.Entrant)]
	public class HobbieController : Controller
	{
        private readonly IOptimalEducationDbContext _dbContext;
        private readonly IApplicationUserManager _userManager;

        public HobbieController(IOptimalEducationDbContext dbContext, IApplicationUserManager userManager)
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

				var userHobbieIdsQuery = 
                    from entrant in _dbContext.Entrants.Include(p=>p.Hobbies).AsNoTracking()
					where entrant.Id==entrantId
					from hobbie in entrant.Hobbies
					select hobbie.Id;

				var userHobbieIds = new HashSet<int>(userHobbieIdsQuery);
				var allHobbies = await _dbContext.Hobbies.AsNoTracking().ToListAsync<Hobbie>();

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
		    var command = new UpdateEntrantHobbieCommand(_dbContext);
            await command.Execute(entrantId, selectedHobbies);
            return RedirectToAction("Index");
		}
        private async Task<int> GetEntrantId()
        {
            var currentUser = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            var entrantClaim = currentUser.Claims.FirstOrDefault(p => p.ClaimType == MyClaimTypes.EntityUserId);
            var entrantId = int.Parse(entrantClaim.ClaimValue);
            return entrantId;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
	}
}
