using Blacksmith.Extensions.Enumerables;
using Blacksmith.Sql.Models;
using Blacksmith.Sql.Queries;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Blacksmith.Sql.Extensions.Queries
{
    public static class QueryExtensions
    {
        public static IEnumerable<IDataRecord> getRecords(this IQuery query, ITransaction transaction)
        {
            return transaction.get(query);
        }
    }
}
