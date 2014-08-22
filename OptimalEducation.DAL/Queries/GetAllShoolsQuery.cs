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
    public class GetAllSchoolsQuery : EFBaseQuery, IQuery<GetAllSchoolsCriterion, Task<IEnumerable<School>>>
    {
        public GetAllSchoolsQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<School>> Ask(GetAllSchoolsCriterion criterion)
        {
            var schools = await _dbContext.Schools
                .AsNoTracking()
                .ToListAsync();
            return schools;
        }
    }

    public class GetAllSchoolsCriterion : ICriterion
    {//ToDO: Add some properthies here or empty
    }

}
