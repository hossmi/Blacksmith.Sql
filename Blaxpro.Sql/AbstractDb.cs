using Blaxpro.Sql.Models;
using System.Data;

namespace Blaxpro.Sql
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