using Interfaces.CQRS;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Interfaces.Logic.Characterizers;
using System.Collections.Generic;

namespace OptimalEducation.Implementation.Logic.Characterizers
{
    public class EducationCharacteristicNamesHelper : IEducationCharacteristicNamesHelper
    {
        private readonly IQueryBuilder _queryBuilder;

        public EducationCharacteristicNamesHelper(IQueryBuilder queryBuilder)
        {
            _queryBuilder = queryBuilder;
        }

        IEnumerable<string> names;
        public IEnumerable<string> Names
        {
            get
            {
                if (names == null)
                {
                    names = _queryBuilder
                        .For<IEnumerable<string>>()
                        .With(new GetEducationCharacterisitcNamesCriterion());
                }
                return names;
            }
        }
    }
}
