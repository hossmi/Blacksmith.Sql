using Blaxpro.Sql.Models;
using System.Collections.Generic;

namespace Blaxpro.Sql
{
    public interface IDbMigrator
    {
        IDictionary<string, IMigration> Migrations { get; }

        void add(IMigration migration);
        IEnumerable<IMigrationStep> getHistory(IDb db);
        bool isInitialized(IDb db);
        IReadOnlyList<IMigrationStep> upgrade(IDb db);
        IReadOnlyList<IMigrationStep> remove(IDb db, string migrationName);
        void initialize(IDb db);
    }
}