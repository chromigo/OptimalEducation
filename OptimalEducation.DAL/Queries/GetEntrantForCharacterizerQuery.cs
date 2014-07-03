using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.DAL.Queries
{
    public class GetEntrantForCharacterizerQuery
    {
        private readonly IOptimalEducationDbContext _dbContext;

        public GetEntrantForCharacterizerQuery(IOptimalEducationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Entrant> Execute(int entrantId)
        {
            var entrant = await _dbContext.Entrants
                .Include(e => e.ParticipationInSchools.Select(h => h.School.Weights))
                .Include(e => e.ParticipationInSections.Select(pse => pse.Section.Weights))
                .Include(e => e.ParticipationInOlympiads.Select(po => po.Olympiad.Weights))
                .Include(e => e.Hobbies.Select(h => h.Weights))
                .Include(e => e.SchoolMarks.Select(sm => sm.SchoolDiscipline.Weights))
                .Include(e => e.UnitedStateExams.Select(use => use.Discipline.Weights))
                .Where(e => e.Id == entrantId)
                .AsNoTracking()
                .SingleAsync();
            return entrant;
        }
    }
}
