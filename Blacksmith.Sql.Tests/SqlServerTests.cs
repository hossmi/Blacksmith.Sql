using System.Collections.Generic;
using System.Linq;
using Blacksmith.Sql.Models;
using Blacksmith.Sql.Tests.Models;
using Blacksmith.Sql.Tests.SqlServerOperations;
using Xunit;

namespace Blacksmith.Sql.Tests
{
    public class SqlServerTests
    {
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

                foreach (var p in ProductsResporitory.Products)
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
