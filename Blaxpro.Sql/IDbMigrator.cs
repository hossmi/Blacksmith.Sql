using System.Collections.Generic;

namespace Blaxpro.Sql
{
    public interface IDbMigrator
    {
        void add(IMigration migration);
        IMigrationResult upgrade(IDb db);
        IEnumerable<IMigrationScript> getScripts(int fromVersion, int toVersion);
    }
}