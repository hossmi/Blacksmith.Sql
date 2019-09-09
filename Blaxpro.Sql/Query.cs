using System.Collections.Generic;
using System.Data;
using Blaxpro.Sql.Models;

namespace Blaxpro.Sql
{
    internal class Query : IQuery
    {
        private IDbTransaction transaction;
        private readonly IDictionary<string, object> parameters;

        public string Statement { get; }
        public IDictionary<string, object> Parameters { get; }

        public Query(IDbTransaction transaction, string query)
        {
            this.Statement = query;
            this.transaction = transaction;
            this.parameters = new Dictionary<string, object>();
        }

        public IEnumerable<T> read<T>() where T : class, new()
        {
            throw new System.NotImplementedException();
        }

        public object readScalar()
        {
            throw new System.NotImplementedException();
        }

        public int write()
        {
            throw new System.NotImplementedException();
        }
    }
}