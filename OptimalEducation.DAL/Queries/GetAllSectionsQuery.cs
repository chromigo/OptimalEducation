using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Interfaces.CQRS;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.DAL.Queries
{
    public class GetAllSectionsQuery : EfBaseQuery, IQuery<GetAllSectionsCriterion, Task<IEnumerable<Section>>>
    {
        public GetAllSectionsQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<Section>> Ask(GetAllSectionsCriterion criterion)
        {
            var sections = await DbContext.Sections
                .AsNoTracking()
                .ToListAsync();
            return sections;
        }
    }

    public class GetAllSectionsCriterion : ICriterion
    {
    }
}