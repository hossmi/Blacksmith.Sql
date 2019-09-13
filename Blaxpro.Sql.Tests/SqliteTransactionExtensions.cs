using Blaxpro.Sql.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Blaxpro.Sql.Extensions.Queries;
using Blaxpro.Sql.Extensions.DbTransactions;

namespace Blaxpro.Sql.Tests
{
    public static class SqliteTransactionExtensions
    {
        public static int dropSqliteProductsTable(this ITransaction transaction)
        {
            return transaction.set(@"DROP TABLE products;");
        }

        public static int createSqliteProductsTable(this ITransaction transaction)
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
    }
}
