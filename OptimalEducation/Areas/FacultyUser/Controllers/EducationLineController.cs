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
using OptimalEducation.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.Logic.Characterizer;
using OptimalEducation.Logic.AnalyticHierarchyProcess;

namespace OptimalEducation.Areas.FacultyUser.Controllers
{
	[Authorize(Roles=Role.Faculty)]
	public class EducationLineController : Controller
	{
        private readonly IOptimalEducationDbContext _dbContext;
        private readonly IApplicationUserManager _userManager;

        public EducationLineController(IOptimalEducationDbContext dbContext, IApplicationUserManager userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
		// GET: /FacultyUser/EducationLine/
		public async Task<ActionResult> Index()
		{
			var facultyId = await GetFacultyId();
			var educationlines = _dbContext.EducationLines
				.Include(e => e.Faculty)
				.Where(e=>e.FacultyId==facultyId);
			return View(await educationlines.ToListAsync());
		}

		// GET: /FacultyUser/EducationLine/Details/5
		public async Task<ActionResult> Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			//Отобразить характеристики текущего направления
            var entrants = await _dbContext.Entrants.Where(p => p.FirstName != "IDEAL")
                                .ToListAsync();
			var facultyId = await GetFacultyId();
			var educationline = await _dbContext.EducationLines
                .Where(p => p.FacultyId == facultyId)
				.FirstOrDefaultAsync(p => p.Id == id);
			if (educationline == null)
			{
				return HttpNotFound();
			}
            //Ориентация направления
            var educationLineCharacterizer = new EducationLineCharacterizer(educationline,new EducationLineCalculationOptions());
            ViewBag.CluserResults = educationLineCharacterizer.CalculateNormSum();

            //Рекомендации по подбору учеников:
            //По методу расстояний
			ViewBag.RecomendationForEducationLine= DistanceCharacterisiticRecomendator.GetRecomendationForEducationLine(educationline, entrants);

            //По методу многокритериального анализа

            //По методу МАИ
            var AHPEducationLineAnalyzer = new AHPEducationLine(educationline, entrants,new AHPEdLineSettings());
            var orderedList = AHPEducationLineAnalyzer.AllCriterionContainer;
            var tempAHPDict = new Dictionary<Entrant, double>();
            foreach (var item in orderedList)
            {
                var edLine = entrants.Find(p => p.Id == item.databaseId);
                tempAHPDict.Add(edLine, item.absolutePriority);
            }
            ViewBag.APHRecomendations = tempAHPDict;

			return View(educationline);
		}

		// GET: /FacultyUser/EducationLine/Create
		public ActionResult Create()
		{
			ViewBag.GeneralEducationLineId = new SelectList(_dbContext.GeneralEducationLines, "Id", "Name");
			return View();
		}

		// POST: /FacultyUser/EducationLine/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create([Bind(Include = "Name,Code,GeneralEducationLineId,EducationForm,Actual,FreePlacesNumber,RequiredSum,PaidPlacesNumber,Price")] EducationLine educationline)
		{
			educationline.FacultyId = await GetFacultyId();
			if (ModelState.IsValid)
			{
				_dbContext.EducationLines.Add(educationline);
				await _dbContext.SaveChangesAsync();
				return RedirectToAction("Index");
			}

			ViewBag.GeneralEducationLineId = new SelectList(_dbContext.GeneralEducationLines, "Id", "Name", educationline.GeneralEducationLineId);
			return View(educationline);
		}

		// GET: /FacultyUser/EducationLine/Edit/5
		public async Task<ActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var facultyId = await GetFacultyId();
			var educationline = await _dbContext.EducationLines
				.Where(p => p.FacultyId == facultyId)
				.FirstOrDefaultAsync(p => p.Id == id);
			if (educationline == null)
			{
				return HttpNotFound();
			}

			ViewBag.GeneralEducationLineId = new SelectList(_dbContext.GeneralEducationLines, "Id", "Name", educationline.GeneralEducationLineId);
			return View(educationline);
		}

		// POST: /FacultyUser/EducationLine/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit([Bind(Include = "Id,FacultyId,Name,Code,GeneralEducationLineId,EducationForm,Actual,FreePlacesNumber,RequiredSum,PaidPlacesNumber,Price")] EducationLine educationline)
		{
			if (ModelState.IsValid)
			{
				var facultyId = await GetFacultyId();
				if (educationline.FacultyId==facultyId)
				{
					_dbContext.Entry(educationline).State = EntityState.Modified;
					await _dbContext.SaveChangesAsync();
					return RedirectToAction("Index");
				}
			}
			ViewBag.GeneralEducationLineId = new SelectList(_dbContext.GeneralEducationLines, "Id", "Name", educationline.GeneralEducationLineId);
			return View(educationline);
		}

		// GET: /FacultyUser/EducationLine/Delete/5
		public async Task<ActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var facultyId = await GetFacultyId();
			var educationLinesFromDb = await _dbContext.EducationLines
				.Where(p => p.FacultyId == facultyId && p.Id == id)
				.SingleOrDefaultAsync();
			if (educationLinesFromDb == null)
			{
				return HttpNotFound();
			}
			return View(educationLinesFromDb);
		}

		// POST: /FacultyUser/EducationLine/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			var facultyId = await GetFacultyId();
			var educationLinesFromDb = await _dbContext.EducationLines
				.Where(p => p.FacultyId == facultyId && p.Id == id)
				.SingleOrDefaultAsync();
			_dbContext.EducationLines.Remove(educationLinesFromDb);
			await _dbContext.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		private async Task<int> GetFacultyId()
		{
			var currentUser = await _userManager.FindByIdAsync(User.Identity.GetUserId());
			var entrantClaim = currentUser.Claims.SingleOrDefault(p => p.ClaimType == MyClaimTypes.EntityUserId);
			var entrantId = int.Parse(entrantClaim.ClaimValue);
			return entrantId;
		}
	}
}
