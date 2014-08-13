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
    public class GetCurrentParticipationInSectionsOfEntrantQuery : EFBaseQuery, IQuery<GetCurrentParticipationInSectionsOfEntrantCriterion, Task<ParticipationInSection>>
    {
        public GetCurrentParticipationInSectionsOfEntrantQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<ParticipationInSection> Ask(GetCurrentParticipationInSectionsOfEntrantCriterion criterion)
        {
            var participationinSection = await _dbContext.ParticipationInSections
                .Include(p => p.Section)
                .Where(p => p.EntrantsId == criterion.EntrantId && p.Id==criterion.Id)
                .AsNoTracking()
                .SingleOrDefaultAsync();// null - HttpNotFound();
            return participationinSection;
        }
    }

    public class GetCurrentParticipationInSectionsOfEntrantCriterion : ICriterion
    {
        public int EntrantId { get; set; }
        public int Id { get; set; }
    }
}
