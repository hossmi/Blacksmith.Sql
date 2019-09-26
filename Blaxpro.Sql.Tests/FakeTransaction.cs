using System.Collections.Generic;
using System.Data;
using Blaxpro.Sql.Models;
using Xunit;

namespace Blaxpro.Sql.Tests
{
    public class FakeTransaction : ITransaction
    {
        private bool disposed;

        public FakeTransaction()
        {
            this.disposed = false;
        }

        public void Dispose()
        {
            Assert.False(this.disposed);
            this.disposed = true;
        }

        public IEnumerable<IDataRecord> get(IQuery query)
        {
            throw new System.NotImplementedException();
        }

        public object getValue(IQuery query)
        {
            throw new System.NotImplementedException();
        }

        public void saveChanges()
        {
        }

        public int set(IQuery query)
        {
            throw new System.NotImplementedException();
        }
    }
}
