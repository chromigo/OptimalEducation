using CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using LightInject;

namespace OptimalEducation
{
    public class QueryBuilder : IQueryBuilder
    {
        private readonly IServiceFactory dependencyResolver;

        public QueryBuilder(IServiceFactory dependencyResolver)
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
            private readonly IServiceFactory dependencyResolver;

            public QueryFor(IServiceFactory dependencyResolver)
            {
                this.dependencyResolver = dependencyResolver;
            }

            public TResult With<TCriterion>(TCriterion criterion) where TCriterion : ICriterion
            {
                return dependencyResolver.GetInstance<IQuery<TCriterion, TResult>>().Ask(criterion);
            }
        }

        #endregion
    }
}
