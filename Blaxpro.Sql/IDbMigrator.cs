using Blaxpro.Sql.Models;
using System.Collections.Generic;

namespace Blaxpro.Sql
{
    public interface IDbMigrator
    {
        MigrationSettings Settings { get; }
        void add(IMigration migration);
        IMigrationResult upgrade(IDb db);
        IEnumerable<IQuery> getQueries(string fromVersion, string toVersion);
        IMigrationStep getLast(IDb db);
    }
}