using System.Data;

namespace Blaxpro.Sql
{
    public interface IDb<T> where T : class, IDbConnection, new()
    {
        ITransaction transact();
    }
}