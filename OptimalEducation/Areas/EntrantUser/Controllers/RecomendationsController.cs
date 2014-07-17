﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.DAL.Models;
using OptimalEducation.Logic.Characterizer;
using OptimalEducation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OptimalEducation.Logic.MulticriterialAnalysis;
using OptimalEducation.Logic.AnalyticHierarchyProcess;
using System.Diagnostics;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
	[Authorize(Roles=Role.Entrant)]
	public class RecomendationsController : Controller
	{
        private readonly IOptimalEducationDbContext _dbContext;
        private readonly IApplicationUserManager _userManager;

        public RecomendationsController(IOptimalEducationDbContext dbContext, IApplicationUserManager userManager)
		{
            _dbContext = dbContext;
            _userManager = userManager;
		}

		// GET: EntrantUser/Recomendations
		public async Task<ActionResult> Index()
		{
			var entrantId = await GetEntrantId();
			var entrant = await _dbContext.Entrants
				.Include(e => e.ParticipationInSchools.Select(h => h.School.Weights))
				.Include(e => e.ParticipationInSections.Select(pse=>pse.Section.Weights))
				.Include(e => e.ParticipationInOlympiads.Select(po => po.Olympiad.Weights))
				.Include(e => e.Hobbies.Select(h => h.Weights))
				.Include(e => e.SchoolMarks.Select(sm => sm.SchoolDiscipline.Weights))
				.Include(e => e.UnitedStateExams.Select(use => use.Discipline.Weights))
				.Where(e => e.Id == entrantId).SingleAsync();

			var educationLines = await _dbContext.EducationLines
				.Include(edl=>edl.EducationLinesRequirements.Select(edlReq=>edlReq.ExamDiscipline.Weights.Select(w=>w.Characterisic)))
				.Include(edl => edl.Faculty.HigherEducationInstitution)
				.Where(p => p.Actual == true && p.Name!="IDEAL")
				.ToListAsync();


			//Рекомендации:
			//По методу сравнения расстояний мд характеристиками
            ViewBag.DistanceRecomendations = DistanceCharacterisiticRecomendator.GetRecomendationForEntrant(entrant, educationLines);
			
			//По методу многокритериального анализа
			var multicriterialAnalyzer = new MulticriterialAnalysis(entrant,educationLines);
            var res = multicriterialAnalyzer.Calculate();
            ViewBag.MulticriterialRecomendations = res;

			//По МАИ
			var AHPUserAnalyzer = new AHPUser(entrant, educationLines, new AHPUserSettings());
			var orderedList = AHPUserAnalyzer.AllCriterionContainer;
			var tempAHPDict = new Dictionary<EducationLine,double>();
			foreach (var item in orderedList)
			{
				var edLine = educationLines.Find(p=>p.Id==item.databaseId);
				tempAHPDict.Add(edLine,item.absolutePriority);
			}
			ViewBag.APHRecomendations = tempAHPDict;
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