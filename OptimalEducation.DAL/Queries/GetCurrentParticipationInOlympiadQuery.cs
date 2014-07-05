using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimalEducation.DAL.Models;
using CQRS;

namespace OptimalEducation.DAL.Queries
{
    public class GetCurrentParticipationInOlympiadQuery : EFBaseQuery, IQuery<GetCurrentParticipationInOlympiadCriterion, Task<ParticipationInOlympiad>>
    {
        public GetCurrentParticipationInOlympiadQuery(IOptimalEducationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ParticipationInOlympiad> Ask(GetCurrentParticipationInOlympiadCriterion criterion)
        {
            var participationinolympiad = await _dbContext.ParticipationInOlympiads
                .Include(p => p.Olympiad)
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.EntrantId == criterion.EntrantId && p.Id == criterion.ParticipationInOlympiadId);
            return participationinolympiad;
        }
    }

    public class GetCurrentParticipationInOlympiadCriterion : ICriterion
    {
        public int EntrantId { get; set; }
        public int ParticipationInOlympiadId { get; set; }
    }
}
