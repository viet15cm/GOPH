﻿using System.Drawing.Printing;
using System.Linq.Expressions;

namespace GOPH.Extensions
{
    public static partial class QueryableExtensions
    {
        public static IOrderedQueryable<T> OrderByMember<T>(this IQueryable<T> source, string memberPath, bool descending)
        {
            var parameter = Expression.Parameter(typeof(T), "item");
            var member = memberPath.Split('.')
                .Aggregate((Expression)parameter, Expression.PropertyOrField);
            var keySelector = Expression.Lambda(member, parameter);
            var methodCall = Expression.Call(
                typeof(Queryable), descending ? "OrderByDescending" : "OrderBy",
                new[] { parameter.Type, member.Type },
                source.Expression, Expression.Quote(keySelector));
            return (IOrderedQueryable<T>)source.Provider.CreateQuery(methodCall);
        }
    }

    //var people = db.People
    //.OrderByMember(sortField, sortDirection == "descending")
    //.Skip((page - 1) * pageSize).Take(pageSize)
    //.ToList();

}
