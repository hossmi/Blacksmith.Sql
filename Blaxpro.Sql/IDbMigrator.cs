using Blaxpro.Sql.Models;

namespace Blaxpro.Sql
{
    public interface IDbMigrator
    {
        void add(IMigration migration);
        IMigrableDb getFor(IDb db);
    }
}