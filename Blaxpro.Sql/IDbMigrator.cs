using Blaxpro.Sql.Models;
using System.Collections.Generic;

namespace Blaxpro.Sql
{
    public interface IDbMigrator
    {
        IMigrationReport getCurrentDbState(IDb db);
        IMigrationReport upgrade(IDb db);
        IMigrationReport downgradeTo(string migrationName, IDb db);
        IEnumerable<IQuery> getQueries(string fromMigration, string toMigration);
        void set(IMigration migration);
    }
}