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
		private OptimalEducationDbContext db = new OptimalEducationDbContext();
		private ApplicationDbContext dbIdentity = new ApplicationDbContext();
		public UserManager<ApplicationUser> UserManager { get; private set; }
		public EducationLineRequirementController()
		{
			UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbIdentity));
		}
		public EducationLineRequirementController(UserManager<ApplicationUser> userManager)
		{
			UserManager = userManager;
		}
		// GET: /FacultyUser/EducationLineRequirement/
		public async Task<ActionResult> Index(int id)
		{
			var facultyId = await GetFacultyId();
			var educationlinerequirements = db.EducationLineRequirements
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
			
			EducationLineRequirement educationlinerequirement = await db.EducationLineRequirements.FindAsync(id);
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
		public async Task<ActionResult> Create(int educationLineId)
		{
			ViewBag.ExamDisciplineId = new SelectList(db.ExamDisciplines, "Id", "Name");
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
				var educationLine= await db.EducationLines.FindAsync(educationlinerequirement.EducationLineId);

				if (educationLine.FacultyId == facultyId)
				{
					db.EducationLineRequirements.Add(educationlinerequirement);
					await db.SaveChangesAsync();
					return RedirectToAction("Index", new { id = educationlinerequirement.EducationLineId });
				}
				else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			ViewBag.ExamDisciplineId = new SelectList(db.ExamDisciplines, "Id", "Name", educationlinerequirement.ExamDisciplineId);
			return View(educationlinerequirement);
		}

		// GET: /FacultyUser/EducationLineRequirement/Edit/5
		public async Task<ActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			EducationLineRequirement educationlinerequirement = await db.EducationLineRequirements.FindAsync(id);
			ViewBag.EducationLineId = educationlinerequirement.EducationLineId;
			if (educationlinerequirement == null)
			{
				return HttpNotFound();
			}
			var facultyId = await GetFacultyId();
			if (educationlinerequirement.EducationLine.FacultyId == facultyId)
			{
				ViewBag.ExamDisciplineId = new SelectList(db.ExamDisciplines, "Id", "Name", educationlinerequirement.ExamDisciplineId);
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
				var educationLine = await db.EducationLines.FindAsync(educationlinerequirement.EducationLineId);
				if (educationLine.FacultyId==facultyId)
				{
					db.Entry(educationlinerequirement).State = EntityState.Modified;
					await db.SaveChangesAsync();
					return RedirectToAction("Index", new { id = educationlinerequirement.EducationLineId });
				}
				else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			ViewBag.ExamDisciplineId = new SelectList(db.ExamDisciplines, "Id", "Name", educationlinerequirement.ExamDisciplineId);
			return View(educationlinerequirement);
		}

		// GET: /FacultyUser/EducationLineRequirement/Delete/5
		public async Task<ActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			EducationLineRequirement educationlinerequirement = await db.EducationLineRequirements.FindAsync(id);
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
			EducationLineRequirement educationlinerequirement = await db.EducationLineRequirements.FindAsync(id);

			var facultyId = await GetFacultyId();
			if (educationlinerequirement.EducationLine.FacultyId == facultyId)
			{
				db.EducationLineRequirements.Remove(educationlinerequirement);
				await db.SaveChangesAsync();
				return RedirectToAction("Index", new { id = educationlinerequirement.EducationLineId });
			}
			else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
		}
		private async Task<int> GetFacultyId()
		{
			var currentUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			var entrantClaim = currentUser.Claims.SingleOrDefault(p => p.ClaimType == MyClaimTypes.EntityUserId);
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
