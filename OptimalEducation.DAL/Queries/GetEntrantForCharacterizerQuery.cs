using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.CQRS;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.DAL.Queries
{
    public class GetEntrantForCharacterizerQuery : EfBaseQuery,
        IQuery<GetEntrantForCharacterizerCriterion, Task<Entrant>>
    {
        public GetEntrantForCharacterizerQuery(IOptimalEducationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Entrant> Ask(GetEntrantForCharacterizerCriterion criterion)
        {
            var entrant = await DbContext.Entrants
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

    public class GetEntrantForCharacterizerCriterion : ICriterion
    {
        public int EntrantId { get; set; }
    }
}