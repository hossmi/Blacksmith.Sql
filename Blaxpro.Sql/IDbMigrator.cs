using Blaxpro.Sql.Models;
using System.Collections.Generic;

namespace Blaxpro.Sql
{
    public interface IDbMigrator
    {
        IDictionary<string, IMigration> Migrations { get; }

        void add(IMigration migration);
        IMigrableDb getFor(IDb db);
    }
}