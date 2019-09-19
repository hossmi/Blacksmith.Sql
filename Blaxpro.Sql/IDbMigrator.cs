using Blaxpro.Sql.Models;
using System.Collections.Generic;

namespace Blaxpro.Sql
{
    public interface IDbMigrator
    {
        MigrationSettings Settings { get; }
        IEnumerable<IQuery> getQueries(string fromVersion, string toVersion);
        IMigrationReport getEnabledMigrations(IDb db);
        IMigrationReport enable<T>(IDb db) where T: class, IMigration, new();
        IMigrationReport disable<T>(IDb db) where T: class, IMigration, new();
    }
}