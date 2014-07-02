using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.DAL.Queries
{
    public class GetParticipationInOlympiadQuery
    {
        private readonly OptimalEducationDbContext _dbContext;

        public GetParticipationInOlympiadQuery(OptimalEducationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ParticipationInOlympiad> Execute(int entrantId, int poId)
        {
            var participationinolympiad = await _dbContext.ParticipationInOlympiads
                .Include(p => p.Olympiad)
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.EntrantId == entrantId && p.Id == poId);
            return participationinolympiad;
        }
    }
}
