using Blaxpro.Sql.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Blaxpro.Sql
{
    public interface ICommandExecutor
    {
        int set(IQuery query);
    }
    public interface ITransaction : IDisposable, ICommandExecutor
    {
        IEnumerable<IDataRecord> get(IQuery query);
        object getValue(IQuery query);
        void saveChanges();
    }
}