using Blaxpro.Sql.Models;
using System.Collections.Generic;

namespace Blaxpro.Sql.Tests
{
    public class V1CreateUsersTableMigration : AbstractMigration
    {
        public static string Create_users_table => "Create users table";

        protected override IEnumerable<IQuery> prv_getUpgrades()
        {
            yield return (Query)@"
CREATE TABLE users
(
    id BIGINT NOT NULL IDENTITY(1,1),
    name nvarchar(256) NOT NULL,
    PRIMARY KEY (id)
);";
        }

        protected override IEnumerable<IQuery> prv_getDowngrades()
        {
            yield return (Query)@"DROP TABLE users;";
        }

        protected override string prv_getName()
        {
            return Create_users_table;
        }
    }
}