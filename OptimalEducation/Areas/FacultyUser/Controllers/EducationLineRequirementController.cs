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

namespace OptimalEducation.Areas.FacultyUser.Controllers
{
    [Authorize(Roles = Role.Faculty)]
    public class EducationLineRequirementController : Controller
    {
        private OptimalEducationDbContext db = new OptimalEducationDbContext();

        // GET: /FacultyUser/EducationLineRequirement/
        public async Task<ActionResult> Index(int id)
        {
            var educationlinerequirements = db.EducationLineRequirements
                .Include(e => e.ExamDiscipline)
                .Where(e=>e.Id==id);
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
            if (educationlinerequirement == null)
            {
                return HttpNotFound();
            }
            return View(educationlinerequirement);
        }

        // GET: /FacultyUser/EducationLineRequirement/Create
        public ActionResult Create()
        {
            ViewBag.EducationLineId = new SelectList(db.EducationLines, "Id", "Code");
            ViewBag.ExamDisciplineId = new SelectList(db.ExamDisciplines, "Id", "Name");
            return View();
        }

        // POST: /FacultyUser/EducationLineRequirement/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="Id,Requirement,EducationLineId,ExamDisciplineId")] EducationLineRequirement educationlinerequirement)
        {
            if (ModelState.IsValid)
            {
                db.EducationLineRequirements.Add(educationlinerequirement);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.EducationLineId = new SelectList(db.EducationLines, "Id", "Code", educationlinerequirement.EducationLineId);
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
            if (educationlinerequirement == null)
            {
                return HttpNotFound();
            }
            ViewBag.EducationLineId = new SelectList(db.EducationLines, "Id", "Code", educationlinerequirement.EducationLineId);
            ViewBag.ExamDisciplineId = new SelectList(db.ExamDisciplines, "Id", "Name", educationlinerequirement.ExamDisciplineId);
            return View(educationlinerequirement);
        }

        // POST: /FacultyUser/EducationLineRequirement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Id,Requirement,EducationLineId,ExamDisciplineId")] EducationLineRequirement educationlinerequirement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(educationlinerequirement).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.EducationLineId = new SelectList(db.EducationLines, "Id", "Code", educationlinerequirement.EducationLineId);
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
            if (educationlinerequirement == null)
            {
                return HttpNotFound();
            }
            return View(educationlinerequirement);
        }

        // POST: /FacultyUser/EducationLineRequirement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            EducationLineRequirement educationlinerequirement = await db.EducationLineRequirements.FindAsync(id);
            db.EducationLineRequirements.Remove(educationlinerequirement);
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
