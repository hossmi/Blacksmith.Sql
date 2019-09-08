using Blaxpro.Sql.Models;
using Blaxpro.Sql.Extensions;
using System.Collections.Generic;
using System.Data.SqlClient;
using Xunit;
using System.Linq;

namespace Blaxpro.Sql.Tests
{
    public class UnitTest1
    {
        private readonly static Product[] initialProducts = new[]
        {
            new Product { Name = "bread", Price = 0.95m},
            new Product { Name = "milk",  Price = 1.34m},
            new Product { Name = "water", Price = 0.99m},
            new Product { Name = "wine",  Price = 13.21m},
        };

        [Fact]
        public void Test1()
        {
            IDb<SqlConnection> db;

            db = new Db<SqlConnection>("");

            using (ITransaction transaction = db.transact())
            {
                IQuery updateQuery, insertQuery;
                IEnumerable<Product> products;

                transaction
                    .beginQuery("DELETE * FROM product")
                    .write();

                insertQuery = transaction.beginQuery("INSERT INTO product (name, price) VALUES (@name, @price)");

                foreach (var p in initialProducts)
                {
                    int affectedRows;

                    affectedRows = insertQuery
                        .setParameters(p)
                        .write();

                    Assert.Equal(1, affectedRows);
                }

                products = transaction
                    .beginQuery(@"SELECT * FROM product WHERE @minPrice <= product.price AND product.price <= @maxPrice")
                    .setParameters(new
                    {
                        minPrice = 100,
                        maxPrice = 200,
                    })
                    .read<Product>()
                    .ToList();

                updateQuery = transaction.beginQuery("UPDATE product SET name = @name");

                foreach (Product product in products)
                {
                    int affectedRows;

                    affectedRows = updateQuery
                        .setParameters(new { name = $"updated_{product.Name}" })
                        .write();

                    Assert.Equal(1, affectedRows);
                }

                transaction.saveChanges();
            }
        }
    }
}
