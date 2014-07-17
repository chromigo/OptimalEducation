using CQRS;
using System.Web.Mvc;

namespace OptimalEducation
{
    public class QueryBuilder : IQueryBuilder
    {
        //private readonly IDependencyResolver dependencyResolver;

        //public QueryBuilder(IDependencyResolver dependencyResolver)
        //{
        //    this.dependencyResolver = dependencyResolver;
        //}

        public IQueryFor<TResult> For<TResult>()
        {
            return new QueryFor<TResult>();//--dependencyResolver
        }

        #region Nested type: QueryFor

        private class QueryFor<TResult> : IQueryFor<TResult>
        {
            //private readonly IDependencyResolver dependencyResolver;

            //public QueryFor(IDependencyResolver dependencyResolver)
            //{
            //    this.dependencyResolver = dependencyResolver;
            //}

            public TResult With<TCriterion>(TCriterion criterion) where TCriterion : ICriterion
            {
                return DependencyResolver.Current.GetService<IQuery<TCriterion, TResult>>().Ask(criterion);
                //return dependencyResolver.GetService<IQuery<TCriterion, TResult>>().Ask(criterion);
            }
        }
        
        #endregion
    }
}
