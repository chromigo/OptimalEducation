using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OptimalEducation.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.Areas.EntrantUser.Controllers
{
    [Authorize(Roles = Role.Entrant)]
	public class SchoolMarkController : Controller
	{
        private OptimalEducationDbContext db = new OptimalEducationDbContext();
        private ApplicationDbContext dbIdentity = new ApplicationDbContext();

		public UserManager<ApplicationUser> UserManager { get; private set; }

		public SchoolMarkController()
		{
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbIdentity));
		}
		public SchoolMarkController(UserManager<ApplicationUser> userManager)
		{
			UserManager = userManager;
		}
		// GET: /EntrantUser/SchoolMark/
		public async Task<ActionResult> Index()
		{
			var examList = await GetUserSchoolMarkAsync();
			return View(examList);
		}
        private async Task<List<SchoolMark>> GetUserSchoolMarkAsync()
        {
            var entrantId = await GetEntrantId();
            var schoolMarks = db.SchoolMarks
                .Include(u => u.SchoolDiscipline)
                .Include(u => u.Entrant)
                .Where(p => p.EntrantId == entrantId);

            return await schoolMarks.ToListAsync();
        }

        // POST: /EntrantUser/UnitedStateExams/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index([Bind(Include = "Result")] IEnumerable<SchoolMark> schoolMark)
        {
            var examList = await GetUserSchoolMarkAsync();
            //Порядок не должен меняться, поэтому обновляем данные так
            for (int i = 0; i < examList.Count; i++)
            {
                examList[i].Result = schoolMark.ElementAt(i).Result;
            }
            schoolMark = examList;//обновляем список новыми значениями(для передачи во View, в случае ошибки)

            //Проверка 
            if (ModelState.IsValid)
            {
                foreach (var exam in schoolMark)
                {
                    db.Entry(exam).State = EntityState.Modified;
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(schoolMark);
        }


        private async Task<int> GetEntrantId()
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
