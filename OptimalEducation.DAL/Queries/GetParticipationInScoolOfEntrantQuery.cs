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
    public class GetParticipationInScoolOfEntrantQuery : EFBaseQuery, IQuery<GetParticipationInScoolOfEntrantCriterion, Task<IEnumerable<ParticipationInSchool>>>
    {
        public GetParticipationInScoolOfEntrantQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<ParticipationInSchool>> Ask(GetParticipationInScoolOfEntrantCriterion criterion)
        {
            var participations = await _dbContext.ParticipationInSchools
                //.Include(p => p.Entrants)
                .Include(p => p.School)
                .Where(p => p.EntrantsId == criterion.EntrantId)
                .AsNoTracking()
                .ToListAsync();
            return participations;
        }
    }

    public class GetParticipationInScoolOfEntrantCriterion : ICriterion
    {
        public int EntrantId { get; set; }
    }
}
