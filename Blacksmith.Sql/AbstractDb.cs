using Blacksmith.Sql.Models;
using System.Data;

namespace Blacksmith.Sql
{
    public abstract class AbstractDb : IDb
    {
        public ITransaction transact()
        {
            return new Transaction(prv_buildConnection);
        }

        protected abstract IDbConnection prv_buildConnection();
    }
}