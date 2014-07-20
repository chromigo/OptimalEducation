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
    public class GetCurrentParticipationInScoolOfEntrantQuery : EFBaseQuery, IQuery<GetCurrentParticipationInScoolOfEntrantCriterion, Task<ParticipationInSchool>>
    {
        public GetCurrentParticipationInScoolOfEntrantQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<ParticipationInSchool> Ask(GetCurrentParticipationInScoolOfEntrantCriterion criterion)
        {
            var participation = await _dbContext.ParticipationInSchools
                .Include(p => p.School)
                .Where(p => p.EntrantsId == criterion.EntrantId && p.Id==criterion.Id)
                .AsNoTracking()
                .SingleOrDefaultAsync();
            return participation;
        }
    }

    public class GetCurrentParticipationInScoolOfEntrantCriterion : ICriterion
    {
        public int EntrantId { get; set; }
        public int Id { get; set; }
    }
}
