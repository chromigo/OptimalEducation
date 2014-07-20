using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using CQRS;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.DAL.Queries
{
    public class GetAllOlympiadsQuery : EFBaseQuery, IQuery<GetAllOlympiadsCriterion, Task<IEnumerable<Olympiad>>>
    {
        public GetAllOlympiadsQuery(IOptimalEducationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Olympiad>> Ask(GetAllOlympiadsCriterion criterion)
        {
			var olympiads =await _dbContext.Olympiads
                .AsNoTracking()
                .ToListAsync();
            return olympiads;
        }

    }
    public class GetAllOlympiadsCriterion:ICriterion
    {
    }
}
