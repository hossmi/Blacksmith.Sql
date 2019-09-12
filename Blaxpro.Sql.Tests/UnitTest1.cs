using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Blaxpro.Sql.Exceptions;
using Blaxpro.Sql.Extensions.DbTransactions;
using Xunit;

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

        private static IDbConnection prv_getConnection()
        {
            return new SqlConnection(@"Data Source=(localdb)\v11.0;Initial Catalog=blaxpro-sql-tests;Integrated Security=True;Pooling=False");
        }

        [Fact]
        public void test1()
        {
            IDb db;

            db = new Db(prv_getConnection);

            using (ITransaction transaction = db.transact())
            {
                try
                {
                    transaction.createProductsTable();
                    transaction.saveChanges();
                }
                catch (DbCommandExecutionException ex)
                {
                }
            }

            using (ITransaction transaction = db.transact())
            {
                IList<Product> products;

                transaction.deleteProducts();

                foreach (var p in initialProducts)
                {
                    int affectedRows;

                    affectedRows = transaction.insertProduct(p);

                    Assert.Equal(1, affectedRows);
                }

                products = transaction
                    .getProductsByPriceRange(1, 20)
                    .ToList();

                Assert.Equal(2, products.Count);

                foreach (Product p in products)
                {
                    int affectedRows;

                    affectedRows = transaction.updateProductName(p.Id, $"updated_{p.Name}");

                    Assert.Equal(1, affectedRows);
                }

                transaction.saveChanges();
            }
        }
    }
}
