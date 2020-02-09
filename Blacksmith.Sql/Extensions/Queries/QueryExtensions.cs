using Blaxpro.Sql.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blaxpro.Sql.Extensions.Queries
{
    public static class QueryExtensions
    {
        public static TQuery setParameters<TQuery>(this TQuery query, object parameters) where TQuery : Query
        {
            IEnumerable<PropertyInfo> properties;

            properties = parameters
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
                .Where(p => p.Name != nameof(IQuery.Parameters) && p.Name != nameof(IQuery.Statement));

            foreach (PropertyInfo p in properties)
                query[p.Name] = p.GetValue(parameters);

            return query;
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
