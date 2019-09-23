using Blaxpro.Sql.Models;

namespace Blaxpro.Sql
{
    public interface IDb
    {
        ITransaction transact();
    }
}