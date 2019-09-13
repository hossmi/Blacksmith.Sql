using System.Collections.Generic;
using System.Data;
using System.Linq;
using Blaxpro.Sql.Models;
using Blaxpro.Sql.Extensions.Queries;
using Blaxpro.Sql.Extensions.DbTransactions;
using Blaxpro.Sql.Exceptions;
using System.Diagnostics;
using System;

namespace Blaxpro.Sql.Tests
{
    public static class TransactionTestExtensions
    {
        public static IDb dropProductsTable(this IDb db)
        {
            prv_migrate(db, prv_dropProductsTable);
            return db;
        }

        public static IDb createProductsTable(this IDb db)
        {
            prv_migrate(db, prv_createProductsTable);
            return db;
        }

        public static IEnumerable<Product> getProductsByPriceRange(this ITransaction transaction
            , decimal minPrice, decimal maxPrice)
        {
            Query query;

            query = @"
SELECT * 
FROM products
WHERE @minPrice <= products.price 
  AND products.price <= @maxPrice";

            query.setParameter(nameof(minPrice), minPrice);
            query.setParameter(nameof(maxPrice), maxPrice);

            return transaction
                .get(query)
                .Select(prv_buildProduct);
        }

        public static int insertProduct(this ITransaction transaction, Product product)
        {
            Query query;

            query = $@"INSERT INTO products (name, price) VALUES (@{nameof(Product.Name)}, @{nameof(Product.Price)})";
            query.setParameters(product);

            return transaction.set(query);
        }

        public static int updateProductName(this ITransaction transaction, long productId, string name)
        {
            Query query;

            query = @"
UPDATE products 
SET name = @name
WHERE id = @id";

            query.setParameter("name", name);
            query.setParameter("id", productId);

            return transaction.set(query);
        }

        public static int deleteProducts(this ITransaction transaction)
        {
            return transaction.set("DELETE FROM products");
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

        private static int prv_createProductsTable(ITransaction transaction)
        {
            Query query;

            query = @"
CREATE TABLE products
(
    id BIGINT NOT NULL IDENTITY(1,1),
    name nvarchar(256) NOT NULL,
    price decimal(10,2) NOT NULL,
    PRIMARY KEY (id)
);";

            return transaction.set(query);
        }

        private static int prv_dropProductsTable(ITransaction transaction)
        {
            return transaction.set(@"DROP TABLE products;");
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
