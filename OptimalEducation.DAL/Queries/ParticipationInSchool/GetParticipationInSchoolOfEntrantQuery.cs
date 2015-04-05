using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.CQRS;

namespace OptimalEducation.DAL.Queries.ParticipationInSchool
{
    public class GetParticipationInSchoolOfEntrantQuery : EfBaseQuery,
        IQuery<GetParticipationInSchoolOfEntrantCriterion, Task<IEnumerable<Models.ParticipationInSchool>>>
    {
        public GetParticipationInSchoolOfEntrantQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<Models.ParticipationInSchool>> Ask(
            GetParticipationInSchoolOfEntrantCriterion criterion)
        {
            var participations = await DbContext.ParticipationInSchools
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