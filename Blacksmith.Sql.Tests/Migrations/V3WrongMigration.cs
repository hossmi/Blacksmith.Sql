using Blaxpro.Sql.Extensions.DbTransactions;
using Blaxpro.Sql.Models;
using System.Collections.Generic;

namespace Blaxpro.Sql.Tests
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

        protected override IEnumerable<IQuery> prv_getUpgrades()
        {
            yield return (Query)"update BOOOOOOOOM;";
        }
    }
}