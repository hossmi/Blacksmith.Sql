using System.Collections.Generic;
using Blacksmith.Sql.Tests.Models;

namespace Blacksmith.Sql.Tests
{
    public static class ProductsResporitory
    {
        public static IEnumerable<Product> Products
        {
            get
            {
                yield return new Product { Name = "bread", Price = 0.95m };
                yield return new Product { Name = "milk", Price = 1.34m };
                yield return new Product { Name = "water", Price = 0.99m };
                yield return new Product { Name = "wine", Price = 13.21m };
            }
        }
    }
}
