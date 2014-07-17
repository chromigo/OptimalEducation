using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQRS;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.DAL.Queries
{
    public class GetEntrantQuery : EFBaseQuery, IQuery<GetEntrantCriterion, Task<Entrant>>
    {
        public GetEntrantQuery(IOptimalEducationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Entrant> Ask(GetEntrantCriterion criterion)
        {
            var entrant = await _dbContext.Entrants
                .Include(e => e.ParticipationInSchools.Select(h => h.School.Weights))
                .Include(e => e.ParticipationInSections.Select(pse => pse.Section.Weights))
                .Include(e => e.ParticipationInOlympiads.Select(po => po.Olympiad.Weights))
                .Include(e => e.Hobbies.Select(h => h.Weights))
                .Include(e => e.SchoolMarks.Select(sm => sm.SchoolDiscipline.Weights))
                .Include(e => e.UnitedStateExams.Select(use => use.Discipline.Weights))
                .Where(e => e.Id == criterion.EntrantId)
                .AsNoTracking()
                .SingleAsync();
            return entrant;
        }
    }

    public class GetEntrantCriterion: ICriterion
    {
        public int EntrantId { get; set; }
    }
}
