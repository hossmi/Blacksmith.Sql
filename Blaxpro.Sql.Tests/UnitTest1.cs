using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Blaxpro.Sql.Exceptions;
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

        private static IDbConnection prv_getSqlServerConnection()
        {
            return new SqlConnection(@"Data Source=(localdb)\v11.0;Initial Catalog=blaxpro-sql-tests;Integrated Security=True;Pooling=False");
        }

        private static IDbConnection prv_getSqliteConnection()
        {
            string databaseFileFullPath;

            databaseFileFullPath = Path.Combine(Environment.CurrentDirectory, "test.sqlite");
            Assert.True(File.Exists(databaseFileFullPath));

            return new SQLiteConnection($@"Data Source={databaseFileFullPath}; Version=3;");
        }

        [Fact]
        public void sqlServer_tests()
        {
            IDb db;

            db = new Db(prv_getSqlServerConnection);

            using (ITransaction transaction = db.transact())
            {
                try
                {
                    transaction.dropProductsTable();
                    transaction.createProductsTable();
                    transaction.saveChanges();
                }
                catch (DbCommandExecutionException ex)
                {
                    Debug.Print(ex.Message);
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

        [Fact]
        public void sqlite_tests()
        {
            IDb db;

            db = new Db(prv_getSqliteConnection);

            using (ITransaction transaction = db.transact())
            {
                try
                {
                    transaction.dropProductsTable();
                    transaction.createSqliteProductsTable();
                }
                catch (DbCommandExecutionException ex)
                {
                    Debug.Print(ex.Message);
                }
                transaction.saveChanges();
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
