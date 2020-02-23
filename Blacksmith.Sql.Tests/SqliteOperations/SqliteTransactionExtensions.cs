using Blacksmith.Sql.Exceptions;
using Blacksmith.Sql.Extensions.Queries;
using Blacksmith.Sql.Models;
using Blacksmith.Sql.Queries;
using Blacksmith.Sql.Queries.Extensions;
using Blacksmith.Sql.Tests.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;

namespace Blacksmith.Sql.Tests.SqliteOperations
{
    public static class SqliteTransactionExtensions
    {
        public static IDb dropProductsTable(this IDb db)
        {
            prv_migrate(db, prv_dropProductsTable);
            return db;
        }

        public static IDb createSqliteProductsTable(this IDb db)
        {
            prv_migrate(db, prv_createProductsTable);
            return db;
        }

        public static int deleteProducts(this ITransaction transaction)
        {
            return transaction.set(new SqlStatement<SQLiteParameter>("DELETE FROM products"));
        }

        public static int insertProduct(this ITransaction transaction, Product product)
        {
            ISqlStatement statement;

            statement = new SqlStatement<SQLiteParameter>($@"
INSERT INTO products (name, price) 
VALUES (@{nameof(Product.Name)}, @{nameof(Product.Price)})");

            statement.setParameters(product);

            return transaction.set(statement);
        }

        public static IEnumerable<Product> getProductsByPriceRange(this ITransaction transaction
            , decimal minPrice, decimal maxPrice)
        {
            return new SqlQuery<SQLiteParameter>()
                .addColumns("SELECT *")
                .addTables("FROM products")
                .addFilter("@minPrice <= products.price", new SQLiteParameter
                {
                    ParameterName = nameof(minPrice),
                    Value = minPrice,
                    DbType = DbType.Double,
                })
                .addFilter("products.price <= @maxPrice", new SQLiteParameter
                {
                    ParameterName = nameof(maxPrice),
                    Value = maxPrice,
                    DbType = DbType.Double,
                })
                .getRecords(transaction)
                .Select(prv_buildProduct);
        }

        public static int updateProductName(this ITransaction transaction, long productId, string name)
        {
            ISqlStatement statement;

            statement = new SqlStatement<SQLiteParameter>(@"
UPDATE products 
SET name = @name
WHERE id = @id");

            statement.setParameter("name", name);
            statement.setParameter("id", productId);

            return transaction.set(statement);
        }

        private static int prv_createProductsTable(ITransaction transaction)
        {
            SqlStatement<SQLiteParameter> statement;

            statement = new SqlStatement<SQLiteParameter>(@"
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

        private static int prv_dropProductsTable(ITransaction transaction)
        {
            return transaction.set(new SqlStatement<SQLiteParameter>(@"DROP TABLE products;"));
        }

        private static Product prv_buildProduct(IDataRecord r)
        {
            return new Product
            {
                Id = (long)r["id"],
                Name = (string)r["name"],
                Price = (decimal)r["price"],
            };
        }
    }
}
