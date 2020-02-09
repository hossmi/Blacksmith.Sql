using System;
using System.Data;

namespace Blaxpro.Sql
{
    public class Db : AbstractDb
    {
        private readonly Func<IDbConnection> connectionBuilder;

        public Db(Func<IDbConnection> connectionBuilder)
        {
            this.connectionBuilder = connectionBuilder;
        }

        protected override IDbConnection prv_buildConnection()
        {
            return this.connectionBuilder();
        }
    }
}