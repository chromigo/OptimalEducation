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
    public class GetAllShoolsQuery : EFBaseQuery, IQuery<GetAllShoolsCriterion, Task<IEnumerable<School>>>
    {
        public GetAllShoolsQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<School>> Ask(GetAllShoolsCriterion criterion)
        {
            var schools = await _dbContext.Schools
                .AsNoTracking()
                .ToListAsync();
            return schools;
        }
    }

    public class GetAllShoolsCriterion : ICriterion
    {
    }
}
