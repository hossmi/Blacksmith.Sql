using Blaxpro.Sql.Models;
using System;
using System.Collections.Generic;

namespace Blaxpro.Sql.Extensions.Queries
{
    public static class QueryExtensions
    {
        public static TQuery setParameters<TQuery>(this TQuery query, object parameters) where TQuery : Query
        {
            throw new NotImplementedException();
        }

        public static TQuery setParameters<TQuery>(this TQuery query, IEnumerable<KeyValuePair<string, object>> parameters) where TQuery : Query
        {
            foreach (var p in parameters)
                query[p.Key] = p.Value;

            return query;
        }

        public static TQuery setParameter<TQuery>(this TQuery query, string name, object value) where TQuery : Query
        {
            query[name] = value;
            return query;
        }

        public static IQuery removeParameters<TQuery>(this TQuery query) where TQuery: Query
        {
            query.clearParameters();
            return query;
        }
    }
}
