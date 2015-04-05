using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.CQRS;

namespace OptimalEducation.DAL.Queries.ParticipationInOlympiad
{
    public class GetAllParticipationInOlympiadOfEntrantQuery : EfBaseQuery,
        IQuery<GetAllParticipationInOlympiadCriterion, Task<IEnumerable<Models.ParticipationInOlympiad>>>
    {
        public GetAllParticipationInOlympiadOfEntrantQuery(IOptimalEducationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Models.ParticipationInOlympiad>> Ask(
            GetAllParticipationInOlympiadCriterion criterion)
        {
            var participationinolympiads = await DbContext.ParticipationInOlympiads
                .Include(p => p.Entrant)
                .Include(p => p.Olympiad)
                .Where(p => p.EntrantId == criterion.EntrantId)
                .AsNoTracking()
                .ToListAsync();
            return participationinolympiads;
        }
    }

    public class GetAllParticipationInOlympiadCriterion : ICriterion
    {
        public int EntrantId { get; set; }
    }
}