using Blaxpro.Sql.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Blaxpro.Sql
{
    public interface ITransaction : IDisposable
    {
        int set(IQuery query);
        IEnumerable<IDataRecord> get(IQuery query);
        object getValue(IQuery query);
        void saveChanges();
    }
}