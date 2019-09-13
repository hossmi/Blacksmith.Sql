using Blaxpro.Sql.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Blaxpro.Sql.Extensions.Queries;
using Blaxpro.Sql.Extensions.DbTransactions;
using System;
using Blaxpro.Sql.Exceptions;
using System.Diagnostics;

namespace Blaxpro.Sql.Tests
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
            Query query;

            query = @"
CREATE TABLE products
(
    id INTEGER PRIMARY KEY,
    name nvarchar(256) NOT NULL,
    price decimal(10,2) NOT NULL
);";

            return transaction.set(query);
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
