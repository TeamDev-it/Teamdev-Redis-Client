using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TeamDev.Redis.LanguageExtensions
{
  public class QueryObject<T> : IQueryable<T>, IQueryable, IQueryProvider, IEnumerable<T>, IEnumerable
  {
    private System.Linq.Expressions.Expression _expression = null;

    public IEnumerator<T> GetEnumerator()
    {
      return null;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return null;
    }

    public Type ElementType
    {
      get { return typeof(T); }
    }

    public System.Linq.Expressions.Expression Expression
    {
      get { return _expression; }
      internal set { _expression = value; }
    }

    public IQueryProvider Provider
    {
      get { return this; }
    }

    public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
    {
      QueryObject<TElement> obj = new QueryObject<TElement>();
      obj.Expression = expression;
      return (IQueryable<TElement>)obj;

    }

    public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
    {
      return CreateQuery<T>(expression);
    }

    public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
    {
      return default(TResult);
    }

    public object Execute(System.Linq.Expressions.Expression expression)
    {
      return null;
    }
  }
}
