using Blacksmith.Sql.Queries;
using System;
using System.Collections.Generic;
using System.Data;

namespace Blacksmith.Sql.Models
{
    public interface ITransaction : IDisposable
    {
        int set(ISqlStatement statement);
        IEnumerable<IDataRecord> get(IQuery query);
        object getValue(IQuery query);
        void saveChanges();
    }
}