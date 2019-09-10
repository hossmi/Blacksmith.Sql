using Blaxpro.Sql.Models;

namespace Blaxpro.Sql.Extensions.DbTransactions
{
    public static class DbTransactionExtensions
    {
        public static int set(this ITransaction transaction, Query query)
        {
            return transaction.set(query as IQuery);
        }
    }
}
