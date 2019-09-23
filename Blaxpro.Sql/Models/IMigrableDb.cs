using System.Collections.Generic;

namespace Blaxpro.Sql.Models
{
    public interface IMigrableDb
    {
        IDb Db { get; }
        IEnumerable<IMigrationStep> History { get; }
        bool IsInitialized { get; }

        IMigrationStep[] upgrade();
        IMigrationStep[] downgradeTo(string migrationName);
        void initialize();
    }
}