using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.CQRS
{
    /// <summary>
    ///     Критерии запроса
    /// </summary>
    public interface ICriterion { }

    public interface IQuery<in TCriterion,out TResult>
      where TCriterion : ICriterion
    {
        TResult Ask(TCriterion criterion);
    }

    /// <summary>
    /// Класс-обертка для упрощения работы с QueryObject (чтобы не вызывать DependencyResolver напрямую)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueryFor<out T>
    {
       T With<TCriterion>(TCriterion criterion) where TCriterion : ICriterion;
    }

    public interface IQueryBuilder
    {
        IQueryFor<TResult> For<TResult>();
    }
}
