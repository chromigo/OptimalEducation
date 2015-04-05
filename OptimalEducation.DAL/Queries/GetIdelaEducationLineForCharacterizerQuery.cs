using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.CQRS;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.DAL.Queries
{
    public class GetIdelaEducationLineForCharacterizerQuery : EfBaseQuery,
        IQuery<GetIdelaEducationLineForCharacterizerCriterion, Task<EducationLine>>
    {
        public GetIdelaEducationLineForCharacterizerQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<EducationLine> Ask(GetIdelaEducationLineForCharacterizerCriterion criterion)
        {
            var edLines = await DbContext.EducationLines
                .Include(
                    edl =>
                        edl.EducationLinesRequirements.Select(
                            edlReq => edlReq.ExamDiscipline.Weights.Select(w => w.Characterisic)))
                .Where(p => p.Name == "IDEAL")
                .AsNoTracking()
                .SingleAsync();
            return edLines;
        }
    }

    public class GetIdelaEducationLineForCharacterizerCriterion : ICriterion
    {
    }
}