using Blacksmith.Sql.Queries;
using Blacksmith.Sql.Exceptions;
using Blacksmith.Sql.Extensions.DbCommands;
using System;
using System.Collections.Generic;
using System.Data;

namespace Blacksmith.Sql.Models
{
    internal class Transaction : ITransaction
    {
        private IDbTransaction transaction;
        private IDbConnection connection;
        private bool disposed;
        private readonly Func<IDbConnection> connectionBuilder;

        public Transaction(Func<IDbConnection> connectionBuilder)
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

        public void saveChanges()
        {
            if (this.transaction == null)
                throw new DbTransactionException("There is no started transaction.");

            this.transaction.Commit();
            this.transaction.Dispose();
            this.transaction = null;
        }

        public IEnumerable<IDataRecord> get(IQuery query)
        {
            return prv_transact(query, prv_getTransaction());
        }

        public object getValue(IQuery query)
        {
            return prv_transact(query, prv_getTransaction(), c => c.ExecuteScalar());
        }

        public int set(ISqlStatement statement)
        {
            return prv_transact(statement, prv_getTransaction(), c => c.ExecuteNonQuery());
        }

        private IDbTransaction prv_getTransaction()
        {
            if (this.connection == null)
            {
                this.connection = this.connectionBuilder()
                    ?? throw new DbTransactionException("Null returned connection calling connectionBuilder delegate.");

                try
                {
                    this.connection.Open();
                }
                catch (Exception ex)
                {
                    throw new DbTransactionException("Error connecting to data base.", ex);
                }
            }

            if (this.transaction == null)
                this.transaction = this.connection.BeginTransaction();

            return this.transaction;
        }

        private static T prv_transact<T>(ISqlStatement statement, IDbTransaction transaction, Func<IDbCommand, T> commandExecutor)
        {
            using (IDbCommand command = transaction.Connection.CreateCommand())
            {
                T result;

                command.Transaction = transaction;
                command.CommandText = statement.Statement;
                command.setParameters(statement.Parameters);

                try
                {
                    result = commandExecutor(command);
                }
                catch (Exception ex)
                {
                    throw new DbCommandExecutionException(ex);
                }
                return result;
            }
        }

        private static IEnumerable<IDataRecord> prv_transact(IQuery query, IDbTransaction transaction)
        {
            using (IDbCommand command = transaction.Connection.CreateCommand())
            {
                IDataReader reader;

                command.Transaction = transaction;
                command.CommandText = query.Statement;
                command.setParameters(query.Parameters);

                try
                {
                    reader = command.ExecuteReader();
                }
                catch (Exception ex)
                {
                    throw new DbCommandExecutionException(ex);
                }

                while (reader.Read())
                    yield return reader;

                reader.Close();
            }
        }
    }
}