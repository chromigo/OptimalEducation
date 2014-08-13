using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;
using CQRS;
using OptimalEducation.DAL.Models;
namespace OptimalEducation.DAL.Queries
{

    public class GetUnitedStateExamsOfEntrantQuery : EFBaseQuery, IQuery<GetUnitedStateExamsOfEntrantCriterion, Task<IEnumerable<UnitedStateExam>>>
    {
        public GetUnitedStateExamsOfEntrantQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<UnitedStateExam>> Ask(GetUnitedStateExamsOfEntrantCriterion criterion)
        {
            var unitedStateExams = await _dbContext.UnitedStateExams
                .Include(u => u.Discipline)
                .AsNoTracking()
                .Where(p => p.EntrantId == criterion.EntrantId)
                .ToListAsync();
            return unitedStateExams;
        }
    }

    public class GetUnitedStateExamsOfEntrantCriterion : ICriterion
    {
        public int EntrantId { get; set; }
    }

}
