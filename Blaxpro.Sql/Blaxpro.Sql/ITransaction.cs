using Blaxpro.Sql.Models;
using System;

namespace Blaxpro.Sql
{
    public interface ITransaction : IDisposable
    {
        IQuery beginQuery(string query);
        void saveChanges();
    }
}