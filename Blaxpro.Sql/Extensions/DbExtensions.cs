using Blaxpro.Sql.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blaxpro.Sql.Extensions
{
    public static class DbExtensions
    {
        public static TQuery setParameters<TQuery>(this TQuery query, object parameters) where TQuery : IQuery
        {
            throw new NotImplementedException();
        }

        public static TQuery setParameters<TQuery>(this TQuery query, IEnumerable<KeyValuePair<string, object>> parameters) where TQuery : IQuery
        {
            foreach (var p in parameters)
                query.Parameters[p.Key] = p.Value;

            return query;
        }

        public static TQuery setParameter<TQuery>(this TQuery query, string name, object value) where TQuery : IQuery
        {
            query.Parameters[name] = value;
            return query;
        }

        public static IQuery clearParameters<TQuery>(this TQuery query) where TQuery: IQuery
        {
            query.Parameters.Clear();
            return query;
        }
    }
}
