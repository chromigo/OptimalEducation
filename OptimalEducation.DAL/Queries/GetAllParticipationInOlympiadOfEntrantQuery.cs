using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.DAL.Queries
{
    public class GetAllParticipationInOlympiadOfEntrantQuery
    {
        private readonly IOptimalEducationDbContext _dbContext;

        public GetAllParticipationInOlympiadOfEntrantQuery(IOptimalEducationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ParticipationInOlympiad>> Execute(int entrantId)
        {
			var participationinolympiads =await _dbContext.ParticipationInOlympiads
				.Include(p => p.Entrant)
				.Include(p => p.Olympiad)
				.Where(p => p.EntrantId == entrantId)
                .AsNoTracking()
                .ToListAsync();
            return participationinolympiads;
        }
    }
}
