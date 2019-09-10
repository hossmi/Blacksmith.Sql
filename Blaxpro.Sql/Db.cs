using System;
using System.Collections.Generic;
using System.Data;
using Blaxpro.Sql.Exceptions;
using Blaxpro.Sql.Extensions.DbCommands;
using Blaxpro.Sql.Models;

namespace Blaxpro.Sql
{
    public class Db : IDb
    {
        private readonly Func<IDbConnection> connectionBuilder;

        public Db(Func<IDbConnection> connectionBuilder)
        {
            this.connectionBuilder = connectionBuilder;
        }

        public ITransaction transact()
        {
            return new PrvTransaction(this.connectionBuilder);
        }

        private class PrvTransaction : ITransaction
        {
            private IDbTransaction transaction;
            private IDbConnection connection;
            private bool disposed;
            private readonly Func<IDbConnection> connectionBuilder;

            public PrvTransaction(Func<IDbConnection> connectionBuilder)
            {
                this.connectionBuilder = connectionBuilder;
            }

            public void Dispose()
            {
                if (this.disposed)
                    throw new DbTransactionException("Connection already disposed.");

                if (this.connection != null)
                {
                    this.connection.Close();
                    this.connection.Dispose();
                    this.connection = null;
                }

                this.disposed = true;
            }

            public IEnumerable<IDataRecord> get(IQuery query)
            {
                return prv_transact(query, prv_executeReader);
            }

            public object getValue(IQuery query)
            {
                return prv_transact(query, command => command.ExecuteNonQuery());
            }

            public void saveChanges()
            {
                if (this.transaction == null)
                    throw new DbTransactionException("There is no started transaction.");

                this.transaction.Commit();
                this.transaction.Dispose();
                this.transaction = null;
            }

            public int set(IQuery query)
            {
                return prv_transact(query, c => c.ExecuteNonQuery());
            }

            private T prv_transact<T>(IQuery query, Func<IDbCommand, T> commandExecutor)
            {
                IDbTransaction transaction;

                transaction = prv_getTransaction();

                using (IDbCommand command = transaction.Connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = query.Statement;
                    command.setParameters(query.Parameters);

                    return commandExecutor(command);
                }
            }

            private IDbTransaction prv_getTransaction()
            {
                if (this.connection == null)
                {
                    this.connection = this.connectionBuilder()
                        ?? throw new DbTransactionException("Null returned connection calling connectionBuilder delegate.");

                    this.connection.Open();
                }

                if (this.transaction == null)
                    this.transaction = this.connection.BeginTransaction();

                return this.transaction;
            }

            private static IEnumerable<IDataRecord> prv_executeReader(IDbCommand command)
            {
                using (IDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                        yield return reader;
            }

        }
    }
}