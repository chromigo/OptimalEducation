using System.Collections.Generic;
using Interfaces.CQRS;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Interfaces.Logic.Characterizers;

namespace OptimalEducation.Implementation.Logic.Characterizers
{
    public class EducationCharacteristicNamesHelper : IEducationCharacteristicNamesHelper
    {
        private IEnumerable<string> _names;
        private readonly IQueryBuilder _queryBuilder;

        public EducationCharacteristicNamesHelper(IQueryBuilder queryBuilder)
        {
            _queryBuilder = queryBuilder;
        }

        public IEnumerable<string> Names => _names ?? (_names = _queryBuilder
                                                .For<IEnumerable<string>>()
                                                .With(new GetEducationCharacterisitcNamesCriterion()));
    }
}