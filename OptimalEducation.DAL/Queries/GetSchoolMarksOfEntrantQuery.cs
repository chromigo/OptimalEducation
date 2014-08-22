using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;
using Interfaces.CQRS;
using OptimalEducation.DAL.Models;
namespace OptimalEducation.DAL.Queries
{
     
    public class GetSchoolMarksOfEntrantQuery : EFBaseQuery, IQuery<GetSchoolMarksOfEntrantCriterion, Task<IEnumerable<SchoolMark>>>
    {
        public GetSchoolMarksOfEntrantQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<SchoolMark>> Ask(GetSchoolMarksOfEntrantCriterion criterion)
        {
            var schoolMarks = await _dbContext.SchoolMarks
                .Include(u => u.SchoolDiscipline)
                .Where(p => p.EntrantId == criterion.EntrantId)
                .AsNoTracking()
                .ToListAsync();
            return schoolMarks;
        }
    }

    public class GetSchoolMarksOfEntrantCriterion : ICriterion
    {
        public int EntrantId { get; set; }
    }

}
