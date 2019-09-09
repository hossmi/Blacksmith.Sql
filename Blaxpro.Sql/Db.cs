using System.Data;
using Blaxpro.Sql.Models;

namespace Blaxpro.Sql
{
    public class Db<T> : IDb<T> where T: class, IDbConnection, new()
    {
        private readonly string connectionString;

        public Db(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public ITransaction transact()
        {
            T connection;

            connection = new T();
            connection.ConnectionString = this.connectionString;

            return new PrvTransaction(connection);
        }

        private class PrvTransaction : ITransaction
        {
            private IDbTransaction transaction;
            private IDbConnection connection;

            public PrvTransaction(IDbConnection connection)
            {
                this.connection = connection;
            }

            public IQuery beginQuery(string query)
            {
                if(this.connection.State != ConnectionState.Open)
                    this.connection.Open();

                if (this.transaction == null)
                    this.transaction = this.connection.BeginTransaction();

                return new Query(this.transaction, query);
            }

            public void Dispose()
            {
                this.connection.Close();
                this.connection.Dispose();
                this.connection = null;
            }

            public void saveChanges()
            {
                this.transaction.Commit();
                this.transaction.Dispose();
                this.transaction = null;
            }
        }
    }
}