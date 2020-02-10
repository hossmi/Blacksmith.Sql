using Blacksmith.Sql.Models;
using System;
using Blacksmith.Sql.Exceptions;
using System.Diagnostics;
using Blacksmith.Sql.Queries.MsSql;

namespace Blacksmith.Sql.Tests.Operations
{
    public static class SqliteTransactionExtensions
    {
        public static IDb createSqliteProductsTable(this IDb db)
        {
            prv_migrate(db, prv_createProductsTable);
            return db;
        }

        private static int prv_createProductsTable(ITransaction transaction)
        {
            SqlStatement statement;

            statement = new SqlStatement(@"
CREATE TABLE products
(
    id INTEGER PRIMARY KEY,
    name nvarchar(256) NOT NULL,
    price decimal(10,2) NOT NULL
);");

            return transaction.set(statement);
        }

        private static void prv_migrate(IDb db, Func<ITransaction, int> transactionDelegate)
        {
            using (ITransaction transaction = db.transact())
            {
                try
                {
                    transactionDelegate(transaction);
                }
                catch (DbCommandExecutionException ex)
                {
                    Debug.Print(ex.Message);
                }
                transaction.saveChanges();
            }
        }
    }
}
