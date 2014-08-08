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
    public class GetParticipationInSectionsOfEntrantQuery : EFBaseQuery, IQuery<GetParticipationInSectionsOfEntrantCriterion, Task<IEnumerable<ParticipationInSection>>>
    {
        public GetParticipationInSectionsOfEntrantQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<ParticipationInSection>> Ask(GetParticipationInSectionsOfEntrantCriterion criterion)
        {
            var participationinSections = await _dbContext.ParticipationInSections
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
