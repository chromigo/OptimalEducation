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

namespace OptimalEducation.Areas.Admin.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class EducationLineRequirementsController : Controller
    {
        private OptimalEducationDbContext db = new OptimalEducationDbContext();

        // GET: Admin/EducationLineRequirements
        public async Task<ActionResult> Index()
        {
            var educationLineRequirements = db.EducationLineRequirements.Include(e => e.EducationLine).Include(e => e.ExamDiscipline);
            return View(await educationLineRequirements.ToListAsync());
        }

        // GET: Admin/EducationLineRequirements/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EducationLineRequirement educationLineRequirement = await db.EducationLineRequirements.FindAsync(id);
            if (educationLineRequirement == null)
            {
                return HttpNotFound();
            }
            return View(educationLineRequirement);
        }

        // GET: Admin/EducationLineRequirements/Create
        public ActionResult Create()
        {
            ViewBag.EducationLineId = new SelectList(db.EducationLines, "Id", "Code");
            ViewBag.ExamDisciplineId = new SelectList(db.ExamDisciplines, "Id", "Name");
            return View();
        }

        // POST: Admin/EducationLineRequirements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Requirement,EducationLineId,ExamDisciplineId")] EducationLineRequirement educationLineRequirement)
        {
            if (ModelState.IsValid)
            {
                db.EducationLineRequirements.Add(educationLineRequirement);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.EducationLineId = new SelectList(db.EducationLines, "Id", "Code", educationLineRequirement.EducationLineId);
            ViewBag.ExamDisciplineId = new SelectList(db.ExamDisciplines, "Id", "Name", educationLineRequirement.ExamDisciplineId);
            return View(educationLineRequirement);
        }

        // GET: Admin/EducationLineRequirements/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EducationLineRequirement educationLineRequirement = await db.EducationLineRequirements.FindAsync(id);
            if (educationLineRequirement == null)
            {
                return HttpNotFound();
            }
            ViewBag.EducationLineId = new SelectList(db.EducationLines, "Id", "Code", educationLineRequirement.EducationLineId);
            ViewBag.ExamDisciplineId = new SelectList(db.ExamDisciplines, "Id", "Name", educationLineRequirement.ExamDisciplineId);
            return View(educationLineRequirement);
        }

        // POST: Admin/EducationLineRequirements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Requirement,EducationLineId,ExamDisciplineId")] EducationLineRequirement educationLineRequirement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(educationLineRequirement).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.EducationLineId = new SelectList(db.EducationLines, "Id", "Code", educationLineRequirement.EducationLineId);
            ViewBag.ExamDisciplineId = new SelectList(db.ExamDisciplines, "Id", "Name", educationLineRequirement.ExamDisciplineId);
            return View(educationLineRequirement);
        }

        // GET: Admin/EducationLineRequirements/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EducationLineRequirement educationLineRequirement = await db.EducationLineRequirements.FindAsync(id);
            if (educationLineRequirement == null)
            {
                return HttpNotFound();
            }
            return View(educationLineRequirement);
        }

        // POST: Admin/EducationLineRequirements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            EducationLineRequirement educationLineRequirement = await db.EducationLineRequirements.FindAsync(id);
            db.EducationLineRequirements.Remove(educationLineRequirement);
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
