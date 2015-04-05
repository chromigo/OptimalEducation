using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.CQRS;

namespace OptimalEducation.DAL.Queries.ParticipationInSection
{
    public class GetCurrentParticipationInSectionsOfEntrantQuery : EfBaseQuery,
        IQuery<GetCurrentParticipationInSectionsOfEntrantCriterion, Task<Models.ParticipationInSection>>
    {
        public GetCurrentParticipationInSectionsOfEntrantQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<Models.ParticipationInSection> Ask(GetCurrentParticipationInSectionsOfEntrantCriterion criterion)
        {
            var participationinSection = await DbContext.ParticipationInSections
                .Include(p => p.Section)
                .Where(p => p.EntrantsId == criterion.EntrantId && p.Id == criterion.Id)
                .AsNoTracking()
                .SingleOrDefaultAsync(); // null - HttpNotFound();
            return participationinSection;
        }
    }

    public class GetCurrentParticipationInSectionsOfEntrantCriterion : ICriterion
    {
        public int EntrantId { get; set; }
        public int Id { get; set; }
    }
}