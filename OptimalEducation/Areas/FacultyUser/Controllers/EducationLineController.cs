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
	[Authorize(Roles=Role.Faculty)]
	public class EducationLineController : Controller
	{
		private OptimalEducationDbContext db = new OptimalEducationDbContext();
		private ApplicationDbContext dbIdentity = new ApplicationDbContext();
		public UserManager<ApplicationUser> UserManager { get; private set; }
		public EducationLineController()
		{
			UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbIdentity));
		}
		public EducationLineController(UserManager<ApplicationUser> userManager)
		{
			UserManager = userManager;
		}
		// GET: /FacultyUser/EducationLine/
		public async Task<ActionResult> Index()
		{
			var facultyId = await GetFacultyId();
			var educationlines = db.EducationLines
				.Include(e => e.Faculty)
				.Include(e => e.GeneralEducationLine)
				.Where(e=>e.Id==facultyId);
			return View(await educationlines.ToListAsync());
		}

		// GET: /FacultyUser/EducationLine/Details/5
		public async Task<ActionResult> Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			EducationLine educationline = await db.EducationLines.FindAsync(id);
			if (educationline == null)
			{
				return HttpNotFound();
			}
			return View(educationline);
		}

		// GET: /FacultyUser/EducationLine/Create
		public ActionResult Create()
		{
			ViewBag.FacultyId = new SelectList(db.Faculties, "Id", "Name");
			ViewBag.GeneralEducationLineId = new SelectList(db.GeneralEducationLines, "Id", "Code");
			return View();
		}

		// POST: /FacultyUser/EducationLine/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create([Bind(Include="Id,GeneralEducationLineId,FacultyId,Code,EducationForm,Name,RequiredSum,Actual,Price,PaidPlacesNumber,FreePlacesNumber")] EducationLine educationline)
		{
			if (ModelState.IsValid)
			{
				db.EducationLines.Add(educationline);
				await db.SaveChangesAsync();
				return RedirectToAction("Index");
			}

			ViewBag.FacultyId = new SelectList(db.Faculties, "Id", "Name", educationline.FacultyId);
			ViewBag.GeneralEducationLineId = new SelectList(db.GeneralEducationLines, "Id", "Code", educationline.GeneralEducationLineId);
			return View(educationline);
		}

		// GET: /FacultyUser/EducationLine/Edit/5
		public async Task<ActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			EducationLine educationline = await db.EducationLines.FindAsync(id);
			if (educationline == null)
			{
				return HttpNotFound();
			}
			ViewBag.FacultyId = new SelectList(db.Faculties, "Id", "Name", educationline.FacultyId);
			ViewBag.GeneralEducationLineId = new SelectList(db.GeneralEducationLines, "Id", "Code", educationline.GeneralEducationLineId);
			return View(educationline);
		}

		// POST: /FacultyUser/EducationLine/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit([Bind(Include="Id,GeneralEducationLineId,FacultyId,Code,EducationForm,Name,RequiredSum,Actual,Price,PaidPlacesNumber,FreePlacesNumber")] EducationLine educationline)
		{
			if (ModelState.IsValid)
			{
				db.Entry(educationline).State = EntityState.Modified;
				await db.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			ViewBag.FacultyId = new SelectList(db.Faculties, "Id", "Name", educationline.FacultyId);
			ViewBag.GeneralEducationLineId = new SelectList(db.GeneralEducationLines, "Id", "Code", educationline.GeneralEducationLineId);
			return View(educationline);
		}

		// GET: /FacultyUser/EducationLine/Delete/5
		public async Task<ActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			EducationLine educationline = await db.EducationLines.FindAsync(id);
			if (educationline == null)
			{
				return HttpNotFound();
			}
			return View(educationline);
		}

		// POST: /FacultyUser/EducationLine/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			EducationLine educationline = await db.EducationLines.FindAsync(id);
			db.EducationLines.Remove(educationline);
			await db.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		private async Task<int> GetFacultyId()
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
