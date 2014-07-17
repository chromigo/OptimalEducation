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
        private readonly IOptimalEducationDbContext _dbContext;
        public EducationLineRequirementsController(IOptimalEducationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: Admin/EducationLineRequirements
        public async Task<ActionResult> Index(int id)
        {
            var educationLineRequirements = _dbContext.EducationLineRequirements
                .Include(e => e.EducationLine)
                .Include(e => e.ExamDiscipline)
                .Where(p=>p.EducationLine.Id==id);
            ViewBag.EducationLineId = id;

            int sum=0;
            foreach (var item in educationLineRequirements)
	        {
		         sum+= item.Requirement;
	        }
            ViewBag.AvaliableSum = _dbContext.EducationLines.Find(id).RequiredSum.Value - sum;

            return View(await educationLineRequirements.ToListAsync());
        }

        // GET: Admin/EducationLineRequirements/Create
        public ActionResult Create(int educationLineId)
        {
            ViewBag.EducationLineId = new SelectList(_dbContext.EducationLines, "Id", "Code",educationLineId );
            ViewBag.ExamDisciplineId = new SelectList(_dbContext.ExamDisciplines, "Id", "Name");

            var educationLineRequirements = _dbContext.EducationLineRequirements
                .Where(p => p.EducationLine.Id == educationLineId);
            int sum = 0;
            foreach (var item in educationLineRequirements)
            {
                sum += item.Requirement;
            }
            ViewBag.AvaliableSum = _dbContext.EducationLines.Find(educationLineId).RequiredSum.Value - sum;

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
                _dbContext.EducationLineRequirements.Add(educationLineRequirement);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.EducationLineId = new SelectList(_dbContext.EducationLines, "Id", "Name", educationLineRequirement.EducationLineId);
            ViewBag.ExamDisciplineId = new SelectList(_dbContext.ExamDisciplines, "Id", "Name", educationLineRequirement.ExamDisciplineId);
            return View(educationLineRequirement);
        }

        // GET: Admin/EducationLineRequirements/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EducationLineRequirement educationLineRequirement = await _dbContext.EducationLineRequirements.FindAsync(id);
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
            EducationLineRequirement educationLineRequirement = await _dbContext.EducationLineRequirements.FindAsync(id);
            _dbContext.EducationLineRequirements.Remove(educationLineRequirement);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
