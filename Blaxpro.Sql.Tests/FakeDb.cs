using Blaxpro.Sql.Models;
using System;

namespace Blaxpro.Sql.Tests
{
    public class FakeDb : IDb
    {
        public event Action<FakeTransaction> BeginTransaction;

        public ITransaction transact()
        {
            FakeTransaction transaction;

            transaction = new FakeTransaction();
            this.BeginTransaction?.Invoke(transaction);

            return transaction;
        }
    }
}
