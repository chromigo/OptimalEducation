using Interfaces.CQRS;
using System.Web.Mvc;

namespace Implimentation.CQRS
{
    public class QueryBuilder : IQueryBuilder
    {
        public IQueryFor<TResult> For<TResult>()
        {
            return new QueryFor<TResult>();
        }

        #region Nested type: QueryFor

        private class QueryFor<TResult> : IQueryFor<TResult>
        {
            public TResult With<TCriterion>(TCriterion criterion) where TCriterion : ICriterion
            {
                return DependencyResolver.Current.GetService<IQuery<TCriterion, TResult>>().Ask(criterion);
            }
        }
        
        #endregion
    }
}
