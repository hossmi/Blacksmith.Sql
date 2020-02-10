using Blacksmith.Extensions.Enumerables;
using Blacksmith.Sql.Models;
using Blacksmith.Sql.Queries;
using Blacksmith.Sql.Queries.MsSql;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Blacksmith.Sql.Extensions.Queries
{
    public static class QueryExtensions
    {
        public static TStatement setParameters<TStatement>(this TStatement query, object parameters) 
            where TStatement : ISqlStatement
        {
            parameters
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
                .forEach(p =>
                {
                    IDbDataParameter parameter;

                    parameter = query.createParameter();
                    parameter.ParameterName = p.Name;
                    parameter.Value = p.GetValue(parameters);
                    query.Parameters.Add(parameter);
                });

            return query;
        }


        public static TStatement setParameters<TStatement>(this TStatement query, IEnumerable<KeyValuePair<string, object>> parameters) 
            where TStatement : ISqlStatement
        {
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                setParameter(query, parameter);
            }
            parameters
                .Select(p => new SqlParameter(p.Key, p.Value))
                .forEach(query.Parameters.Add);

            return query;
        }

        public static TStatement setParameter<TStatement>(this TStatement query, string name, object value) 
            where TStatement : ISqlStatement
        {
            IDbDataParameter parameter;

            parameter = query.createParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            query.Parameters.Add(parameter);

            return query;
        }

        public static TQuery clearParameters<TQuery>(this TQuery query) where TQuery: IQuery
        {
            query.Parameters.Clear();
            return query;
        }

        public static IEnumerable<IDataRecord> getRecords(this IQuery query, ITransaction transaction)
        {
            return transaction.get(query);
        }
    }
}
