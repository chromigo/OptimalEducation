using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.CQRS;

namespace OptimalEducation.DAL.Queries.ParticipationInSection
{
    public class GetParticipationInSectionsOfEntrantQuery : EfBaseQuery,
        IQuery<GetParticipationInSectionsOfEntrantCriterion, Task<IEnumerable<Models.ParticipationInSection>>>
    {
        public GetParticipationInSectionsOfEntrantQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<Models.ParticipationInSection>> Ask(
            GetParticipationInSectionsOfEntrantCriterion criterion)
        {
            var participationinSections = await DbContext.ParticipationInSections
                .Include(p => p.Section)
                .Where(p => p.EntrantsId == criterion.EntrantId)
                .AsNoTracking()
                .ToListAsync();
            return participationinSections;
        }
    }

    public class GetParticipationInSectionsOfEntrantCriterion : ICriterion
    {
        public int EntrantId { get; set; }
    }
}