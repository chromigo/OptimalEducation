using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;
using CQRS;
using OptimalEducation.DAL.Models;
namespace OptimalEducation.DAL.Queries
{

    public class GetEducationCharacterisitcNamesQuery : EFBaseQuery, IQuery<GetEducationCharacterisitcNamesCriterion, Task<IEnumerable<string>>>
    {
        public GetEducationCharacterisitcNamesQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<string>> Ask(GetEducationCharacterisitcNamesCriterion criterion)
        {
            var characterisitcNames = await _dbContext.Characteristics
                .Where(p => p.Type == CharacteristicType.Education)
                .Select(p => p.Name)
                .AsNoTracking()
                .ToListAsync();
            return characterisitcNames;
        }
    }

    public class GetEducationCharacterisitcNamesCriterion : ICriterion
    {
    }

}
