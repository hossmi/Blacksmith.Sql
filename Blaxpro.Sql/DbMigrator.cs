using System.Collections.Generic;

namespace Blaxpro.Sql
{
    public class DbMigrator : IDbMigrator
    {
        public void add(IMigration migration)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IMigrationScript> getScripts(int fromVersion, int toVersion)
        {
            throw new System.NotImplementedException();
        }

        public IMigrationResult upgrade(IDb db)
        {
            throw new System.NotImplementedException();
        }
    }
}