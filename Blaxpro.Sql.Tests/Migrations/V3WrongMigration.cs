using Blaxpro.Sql.Extensions.DbTransactions;
using Blaxpro.Sql.Models;
using System.Collections.Generic;

namespace Blaxpro.Sql.Tests
{
    public class V3WrongMigration : IMigration
    {
        public IEnumerable<IMigration> getDependencies()
        {
            yield return new V2AddUserBirthDateColumnMigration();
        }

        public IEnumerable<IQuery> getDowngrades()
        {
            yield break;
        }

        public IEnumerable<IQuery> getUpgrades()
        {
            yield return (Query)"update BOOOOOOOOM;";
        }
    }
}