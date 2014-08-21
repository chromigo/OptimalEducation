using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces.CQRS;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.DAL.Queries
{
    public class GetParticipationInSchoolOfEntrantQuery : EFBaseQuery, IQuery<GetParticipationInSchoolOfEntrantCriterion, Task<IEnumerable<ParticipationInSchool>>>
    {
        public GetParticipationInSchoolOfEntrantQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<ParticipationInSchool>> Ask(GetParticipationInSchoolOfEntrantCriterion criterion)
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

    public class GetParticipationInSchoolOfEntrantCriterion : ICriterion
    {
        public int EntrantId { get; set; }
    }
}
