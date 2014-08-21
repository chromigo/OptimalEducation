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
    public class GetCurrentParticipationInSchoolOfEntrantQuery : EFBaseQuery, IQuery<GetCurrentParticipationInSchoolOfEntrantCriterion, Task<ParticipationInSchool>>
    {
        public GetCurrentParticipationInSchoolOfEntrantQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<ParticipationInSchool> Ask(GetCurrentParticipationInSchoolOfEntrantCriterion criterion)
        {
            var participation = await _dbContext.ParticipationInSchools
                .Include(p => p.School)
                .Where(p => p.EntrantsId == criterion.EntrantId && p.Id==criterion.Id)
                .AsNoTracking()
                .SingleOrDefaultAsync();
            return participation;
        }
    }

    public class GetCurrentParticipationInSchoolOfEntrantCriterion : ICriterion
    {
        public int EntrantId { get; set; }
        public int Id { get; set; }
    }
}
