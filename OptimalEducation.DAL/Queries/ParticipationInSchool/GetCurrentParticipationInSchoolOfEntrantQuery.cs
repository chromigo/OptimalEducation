using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.CQRS;

namespace OptimalEducation.DAL.Queries.ParticipationInSchool
{
    public class GetCurrentParticipationInSchoolOfEntrantQuery : EfBaseQuery,
        IQuery<GetCurrentParticipationInSchoolOfEntrantCriterion, Task<Models.ParticipationInSchool>>
    {
        public GetCurrentParticipationInSchoolOfEntrantQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<Models.ParticipationInSchool> Ask(GetCurrentParticipationInSchoolOfEntrantCriterion criterion)
        {
            var participation = await DbContext.ParticipationInSchools
                .Include(p => p.School)
                .Where(p => p.EntrantsId == criterion.EntrantId && p.Id == criterion.Id)
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