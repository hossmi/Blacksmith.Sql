using System.Data;

namespace Blaxpro.Sql
{
    public class Db<T> : IDb<T> where T: class, IDbConnection, new()
    {
        private readonly string connectionString;

        public Db(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public ITransaction transact()
        {
            throw new System.NotImplementedException();
        }
    }
}