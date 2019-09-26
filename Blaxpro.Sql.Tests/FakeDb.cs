using Blaxpro.Sql.Models;

namespace Blaxpro.Sql.Tests
{
    public class FakeDb : IDb
    {
        public ITransaction transact()
        {
            return new FakeTransaction();
        }
    }
}
