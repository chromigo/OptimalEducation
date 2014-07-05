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
    public class GetAllOlympiadsQuery : EFBaseQuery, IQuery<GetAllOlympiads, Task<IEnumerable<Olympiad>>>
    {
        public GetAllOlympiadsQuery(IOptimalEducationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Olympiad>> Ask(GetAllOlympiads criterion)
        {
			var olympiads =await _dbContext.Olympiads
                .AsNoTracking()
                .ToListAsync();
            return olympiads;
        }

    }
    public class GetAllOlympiads:ICriterion
    {
    }
}
