using Blacksmith.Sql.Models;

namespace Blacksmith.Sql
{
    public interface IDb
    {
        ITransaction transact();
    }
}