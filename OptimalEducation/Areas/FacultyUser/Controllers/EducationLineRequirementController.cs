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

namespace OptimalEducation.Areas.FacultyUser.Controllers
{
	[Authorize(Roles = Role.Faculty)]
	public class EducationLineRequirementController : Controller
	{
		private readonly OptimalEducationDbContext _dbContext;
        private readonly IApplicationUserManager _userManager;

        public EducationLineRequirementController(OptimalEducationDbContext dbContext, IApplicationUserManager userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
		// GET: /FacultyUser/EducationLineRequirement/
		public async Task<ActionResult> Index(int id)
		{
			var facultyId = await GetFacultyId();
			var educationlinerequirements = _dbContext.EducationLineRequirements
				.Include(e => e.ExamDiscipline)
				.Where(e => e.EducationLineId == id && e.EducationLine.FacultyId==facultyId);
			ViewBag.EducationLineId = id;
			return View(await educationlinerequirements.ToListAsync());
		}

		// GET: /FacultyUser/EducationLineRequirement/Details/5
		public async Task<ActionResult> Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			
			EducationLineRequirement educationlinerequirement = await _dbContext.EducationLineRequirements.FindAsync(id);
			ViewBag.EducationLineId = educationlinerequirement.EducationLineId;
			if (educationlinerequirement == null)
			{
				return HttpNotFound();
			}
			var facultyId = await GetFacultyId();
			if (educationlinerequirement.EducationLine.FacultyId == facultyId)
			{
				return View(educationlinerequirement);
			}
			else return HttpNotFound();
		}

		// GET: /FacultyUser/EducationLineRequirement/Create
		public ActionResult Create(int educationLineId)
		{
			ViewBag.ExamDisciplineId = new SelectList(_dbContext.ExamDisciplines, "Id", "Name");
			ViewBag.EducationLineId = educationLineId;
			return View();
		}

		// POST: /FacultyUser/EducationLineRequirement/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create([Bind(Include="Requirement,EducationLineId,ExamDisciplineId")] EducationLineRequirement educationlinerequirement)
		{
			ViewBag.EducationLineId = educationlinerequirement.EducationLineId;
			if (ModelState.IsValid)
			{
				var facultyId = await GetFacultyId();
				var educationLine= await _dbContext.EducationLines.FindAsync(educationlinerequirement.EducationLineId);

				if (educationLine.FacultyId == facultyId)
				{
					_dbContext.EducationLineRequirements.Add(educationlinerequirement);
					await _dbContext.SaveChangesAsync();
					return RedirectToAction("Index", new { id = educationlinerequirement.EducationLineId });
				}
				else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			ViewBag.ExamDisciplineId = new SelectList(_dbContext.ExamDisciplines, "Id", "Name", educationlinerequirement.ExamDisciplineId);
			return View(educationlinerequirement);
		}

		// GET: /FacultyUser/EducationLineRequirement/Edit/5
		public async Task<ActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			EducationLineRequirement educationlinerequirement = await _dbContext.EducationLineRequirements.FindAsync(id);
			ViewBag.EducationLineId = educationlinerequirement.EducationLineId;
			if (educationlinerequirement == null)
			{
				return HttpNotFound();
			}
			var facultyId = await GetFacultyId();
			if (educationlinerequirement.EducationLine.FacultyId == facultyId)
			{
				ViewBag.ExamDisciplineId = new SelectList(_dbContext.ExamDisciplines, "Id", "Name", educationlinerequirement.ExamDisciplineId);
				return View(educationlinerequirement);
			}
			else return HttpNotFound();
		}

		// POST: /FacultyUser/EducationLineRequirement/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit([Bind(Include="Id,Requirement,EducationLineId,ExamDisciplineId")] EducationLineRequirement educationlinerequirement)
		{
			ViewBag.EducationLineId = educationlinerequirement.EducationLineId;
			if (ModelState.IsValid)
			{
				var facultyId = await GetFacultyId();
				var educationLine = await _dbContext.EducationLines.FindAsync(educationlinerequirement.EducationLineId);
				if (educationLine.FacultyId==facultyId)
				{
					_dbContext.Entry(educationlinerequirement).State = EntityState.Modified;
					await _dbContext.SaveChangesAsync();
					return RedirectToAction("Index", new { id = educationlinerequirement.EducationLineId });
				}
				else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			ViewBag.ExamDisciplineId = new SelectList(_dbContext.ExamDisciplines, "Id", "Name", educationlinerequirement.ExamDisciplineId);
			return View(educationlinerequirement);
		}

		// GET: /FacultyUser/EducationLineRequirement/Delete/5
		public async Task<ActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			EducationLineRequirement educationlinerequirement = await _dbContext.EducationLineRequirements.FindAsync(id);
			ViewBag.EducationLineId = educationlinerequirement.EducationLineId;
			if (educationlinerequirement == null)
			{
				return HttpNotFound();
			}
			var facultyId = await GetFacultyId();
			if (educationlinerequirement.EducationLine.FacultyId == facultyId)
			{
				return View(educationlinerequirement);
			}
			else return HttpNotFound();
		}

		// POST: /FacultyUser/EducationLineRequirement/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			EducationLineRequirement educationlinerequirement = await _dbContext.EducationLineRequirements.FindAsync(id);

			var facultyId = await GetFacultyId();
			if (educationlinerequirement.EducationLine.FacultyId == facultyId)
			{
				_dbContext.EducationLineRequirements.Remove(educationlinerequirement);
				await _dbContext.SaveChangesAsync();
				return RedirectToAction("Index", new { id = educationlinerequirement.EducationLineId });
			}
			else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
