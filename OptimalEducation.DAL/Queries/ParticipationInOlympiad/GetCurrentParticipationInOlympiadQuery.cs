using System.Data.Entity;
using System.Threading.Tasks;
using Interfaces.CQRS;

namespace OptimalEducation.DAL.Queries.ParticipationInOlympiad
{
    public class GetCurrentParticipationInOlympiadQuery : EfBaseQuery,
        IQuery<GetCurrentParticipationInOlympiadCriterion, Task<Models.ParticipationInOlympiad>>
    {
        public GetCurrentParticipationInOlympiadQuery(IOptimalEducationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Models.ParticipationInOlympiad> Ask(GetCurrentParticipationInOlympiadCriterion criterion)
        {
            var participationinolympiad = await DbContext.ParticipationInOlympiads
                .Include(p => p.Olympiad)
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    p => p.EntrantId == criterion.EntrantId && p.Id == criterion.ParticipationInOlympiadId);
            return participationinolympiad;
        }
    }

    public class GetCurrentParticipationInOlympiadCriterion : ICriterion
    {
        public int EntrantId { get; set; }
        public int ParticipationInOlympiadId { get; set; }
    }
}