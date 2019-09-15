using Blaxpro.Sql.Models;
using System.Collections.Generic;

namespace Blaxpro.Sql.Tests
{
    public class V2AddUserBirthDateColumnMigration : IMigration
    {
        public static string AddUsersBirthdateColumn => "Add Users Birthdate Column";
        public string Name => AddUsersBirthdateColumn;

        public IEnumerable<IMigration> getDependencies()
        {
            yield return new V1CreateUsersTableMigration();
        }

        public IEnumerable<IQuery> getDowngrades()
        {
            yield return (Query)"ALTER TABLE users DROM COLUMN birthdate;";
        }

        public IEnumerable<IQuery> getUpgrades()
        {
            yield return (Query)"ALTER TABLE users ADD COLUMN birthdate DATETIME NULL;";
        }
    }
}