using Blaxpro.Sql.Models;
using System.Collections.Generic;

namespace Blaxpro.Sql
{
    public interface IDbMigrator
    {
        MigrationSettings Settings { get; }
        IMigrationReport getEnabledMigrations(IDb db);
        IMigrationReport enable<T>(IDb db) where T: class, IMigration, new();
        IMigrationReport disable<T>(IDb db) where T: class, IMigration, new();
        IEnumerable<IQuery> getQueries<TMigrationFrom, TMigrationTo>()
            where TMigrationFrom : class, IMigration, new()
            where TMigrationTo : class, IMigration, new();
    }
}