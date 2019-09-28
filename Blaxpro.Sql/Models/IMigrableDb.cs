using System.Collections.Generic;

namespace Blaxpro.Sql.Models
{
    public interface IMigrableDb
    {
        IDb Db { get; }
        IEnumerable<IMigrationStep> History { get; }
        bool IsInitialized { get; }

        IReadOnlyList<IMigrationStep> upgrade();
        IReadOnlyList<IMigrationStep> remove(string migrationName);
        void initialize();
    }
}