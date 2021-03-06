﻿using System;
using System.Collections.Generic;
using System.Data;
using Blacksmith.Sql.Models;
using Blacksmith.Sql.Queries;
using Xunit;

namespace Blacksmith.Sql.Tests.Fakes
{
    public class FakeTransaction : ITransaction
    {
        private bool disposed;

        public event Func<ISqlStatement, int> InvokedSet;

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

        public int set(ISqlStatement sqlStatement)
        {
            return this.InvokedSet?.Invoke(sqlStatement) ?? 0;
        }
    }
}
