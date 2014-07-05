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
    public class GetCurrentParticipationInOlympiadQuery : EFBaseQuery, IQuery<GetCurrentParticipationInOlympiad, Task<ParticipationInOlympiad>>
    {
        public GetCurrentParticipationInOlympiadQuery(IOptimalEducationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ParticipationInOlympiad> Ask(GetCurrentParticipationInOlympiad criterion)
        {
            var participationinolympiad = await _dbContext.ParticipationInOlympiads
                .Include(p => p.Olympiad)
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.EntrantId == criterion.EntrantId && p.Id == criterion.ParticipationInOlympiadId);
            return participationinolympiad;
        }
    }

    public class GetCurrentParticipationInOlympiad : ICriterion
    {
        public int EntrantId { get; set; }
        public int ParticipationInOlympiadId { get; set; }
    }
}
