using System.Collections.Generic;
using System.Data;
using System.Linq;
using Blaxpro.Sql.Models;
using Blaxpro.Sql.Extensions.Queries;
using Blaxpro.Sql.Extensions.DbTransactions;

namespace Blaxpro.Sql.Tests
{
    public static class TransactionTestExtensions
    {
        public static int createProductsTable(this ITransaction transaction)
        {
            Query query;

            query = @"
CREATE TABLE products
(
    id int NOT NULL IDENTITY(1,1),
    name nvarchar(256) NOT NULL,
    price decimal(10,2) NOT NULL,
    PRIMARY KEY (id)
);";

            return transaction.set(query);
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

        public static int updateProductName(this ITransaction transaction, int productId, string name)
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

        private static Product prv_buildProduct(IDataRecord r)
        {
            return new Product
            {
                Id = (int)r["id"],
                Name = (string)r["name"],
                Price = (decimal)r["price"],
            };
        }
    }
}
