using Blacksmith.Sql.Models;
using Blacksmith.Sql.Queries;
using Blacksmith.Sql.Queries.Extensions;
using Blacksmith.Sql.Queries.MsSql;
using System.Collections.Generic;

namespace Blacksmith.Sql.Tests
{
    public class V3WrongMigration : AbstractMigration
    {
        public static string WrongMigration => "Booooooom";


        protected override IEnumerable<IMigration> prv_getDependencies()
        {
            yield return new V2AddUserBirthDateColumnMigration();
        }

        protected override string prv_getName()
        {
            return WrongMigration;
        }

        protected override IEnumerable<ISqlStatement> prv_getUpgrades()
        {
            yield return new SqlStatement("update BOOOOOOOOM;");
        }
    }
}