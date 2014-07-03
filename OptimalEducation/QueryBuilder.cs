using CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OptimalEducation
{
    public class QueryBuilder : IQueryBuilder
    {
        private readonly IDependencyResolver dependencyResolver;

        public QueryBuilder(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public IQueryFor<TResult> For<TResult>()
        {
            return new QueryFor<TResult>(dependencyResolver);
        }

        #region Nested type: QueryFor

        private class QueryFor<TResult> : IQueryFor<TResult>
        {
            private readonly IDependencyResolver dependencyResolver;

            public QueryFor(IDependencyResolver dependencyResolver)
            {
                this.dependencyResolver = dependencyResolver;
            }

            public TResult With<TCriterion>(TCriterion criterion) where TCriterion : ICriterion
            {
                return dependencyResolver.GetService<IQuery<TCriterion, TResult>>().Ask(criterion);
            }
        }

        #endregion
    }
}
