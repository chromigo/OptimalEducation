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
    public class EducationLinesController : Controller
    {
        private OptimalEducationDbContext db = new OptimalEducationDbContext();

        // GET: Admin/EducationLines
        public async Task<ActionResult> Index()
        {
            var educationLines = db.EducationLines.Include(e => e.Faculty).Include(e => e.GeneralEducationLine);
            return View(await educationLines.ToListAsync());
        }

        // GET: Admin/EducationLines/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EducationLine educationLine = await db.EducationLines.FindAsync(id);
            if (educationLine == null)
            {
                return HttpNotFound();
            }
            return View(educationLine);
        }

        // GET: Admin/EducationLines/Create
        public ActionResult Create()
        {
            SelectedListShow();
            return View();
        }

        // POST: Admin/EducationLines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,GeneralEducationLineId,FacultyId,Code,EducationForm,Name,RequiredSum,Actual,Price,PaidPlacesNumber,FreePlacesNumber")] EducationLine educationLine)
        {
            if (ModelState.IsValid)
            {
                db.EducationLines.Add(educationLine);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            SelectedListShow();

            return View(educationLine);
        }

        // GET: Admin/EducationLines/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EducationLine educationLine = await db.EducationLines.FindAsync(id);
            if (educationLine == null)
            {
                return HttpNotFound();
            }

            SelectedListShow();

            return View(educationLine);
        }

        private void SelectedListShow()
        {
            var facultyList = from faculty in db.Faculties.Include(p => p.HigherEducationInstitution)
                              select new { Id = faculty.Id, Name = faculty.HigherEducationInstitution.Name + " - " + faculty.Name };
            ViewBag.FacultyId = new SelectList(facultyList, "Id", "Name");
            ViewBag.GeneralEducationLineId = new SelectList(db.GeneralEducationLines, "Id", "Name");//select here id or code, or Name
        }

        // POST: Admin/EducationLines/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,GeneralEducationLineId,FacultyId,Code,EducationForm,Name,RequiredSum,Actual,Price,PaidPlacesNumber,FreePlacesNumber")] EducationLine educationLine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(educationLine).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            SelectedListShow();

            return View(educationLine);
        }

        // GET: Admin/EducationLines/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EducationLine educationLine = await db.EducationLines.FindAsync(id);
            if (educationLine == null)
            {
                return HttpNotFound();
            }
            return View(educationLine);
        }

        // POST: Admin/EducationLines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            EducationLine educationLine = await db.EducationLines.FindAsync(id);
            db.EducationLines.Remove(educationLine);
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
