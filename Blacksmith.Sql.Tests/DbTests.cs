using System.Collections.Generic;
using System.Linq;
using Blacksmith.Sql.Models;
using Blacksmith.Sql.Tests.Models;
using Blacksmith.Sql.Tests.Operations;
using Xunit;

namespace Blacksmith.Sql.Tests
{
    public class DbTests
    {
        private readonly static Product[] initialProducts = new[]
        {
            new Product { Name = "bread", Price = 0.95m},
            new Product { Name = "milk",  Price = 1.34m},
            new Product { Name = "water", Price = 0.99m},
            new Product { Name = "wine",  Price = 13.21m},
        };

        [Fact]
        public void sqlServer_tests()
        {
            IDb db;

            db = new Db(Connections.getSqlServerConnection);
            db.dropProductsTable();
            db.createProductsTable();

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

        [Fact]
        public void sqlite_tests()
        {
            IDb db;

            db = new Db(Connections.getSqliteConnection);
            db.dropProductsTable();
            db.createSqliteProductsTable();

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
