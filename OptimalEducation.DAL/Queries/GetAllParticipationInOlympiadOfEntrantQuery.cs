using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimalEducation.DAL.Models;
using CQRS;

namespace OptimalEducation.DAL.Queries
{
    public class GetAllParticipationInOlympiadOfEntrantQuery :  EFBaseQuery, IQuery<GetAllParticipationInOlympiad, Task<IEnumerable<ParticipationInOlympiad>>>
    {
        public GetAllParticipationInOlympiadOfEntrantQuery(IOptimalEducationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<ParticipationInOlympiad>> Ask(GetAllParticipationInOlympiad criterion)
        {
			var participationinolympiads =await _dbContext.ParticipationInOlympiads
				.Include(p => p.Entrant)
				.Include(p => p.Olympiad)
				.Where(p => p.EntrantId == criterion.EntrantId)
                .AsNoTracking()
                .ToListAsync();
            return participationinolympiads;
        }
    }

    public class GetAllParticipationInOlympiad : ICriterion
    {
        public int EntrantId { get; set; }
    }
}
