using Interfaces.CQRS;
using OptimalEducation.DAL.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Implementation.Logic.Characterizers
{
    public class EducationCharacteristicNamesHelper
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
