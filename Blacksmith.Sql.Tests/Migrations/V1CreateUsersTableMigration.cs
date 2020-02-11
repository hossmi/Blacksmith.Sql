using Blacksmith.Sql.Models;
using Blacksmith.Sql.Queries;
using Blacksmith.Sql.Queries.MsSql;
using System.Collections.Generic;

namespace Blacksmith.Sql.Tests
{
    public class V1CreateUsersTableMigration : AbstractMigration
    {
        public static string Create_users_table => "Create users table";

        protected override IEnumerable<ISqlStatement> prv_getUpgrades()
        {
            yield return new SqlStatement(@"
CREATE TABLE users
(
    id BIGINT NOT NULL IDENTITY(1,1),
    name nvarchar(256) NOT NULL,
    PRIMARY KEY (id)
);");
        }

        protected override IEnumerable<ISqlStatement> prv_getDowngrades()
        {
            yield return new SqlStatement(@"DROP TABLE users;");
        }

        protected override string prv_getName()
        {
            return Create_users_table;
        }
    }
}