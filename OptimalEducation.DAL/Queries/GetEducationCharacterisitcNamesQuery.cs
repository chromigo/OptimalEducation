using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Interfaces.CQRS;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.DAL.Queries
{
    public class GetEducationCharacterisitcNamesQuery : EfBaseQuery,
        IQuery<GetEducationCharacterisitcNamesCriterion, IEnumerable<string>>
    {
        public GetEducationCharacterisitcNamesQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public IEnumerable<string> Ask(GetEducationCharacterisitcNamesCriterion criterion)
        {
            var characterisitcNames = DbContext.Characteristics
                .Where(p => p.Type == CharacteristicType.Education)
                .Select(p => p.Name)
                .AsNoTracking()
                .ToList();
            return characterisitcNames;
        }
    }

    public class GetEducationCharacterisitcNamesCriterion : ICriterion
    {
    }
}