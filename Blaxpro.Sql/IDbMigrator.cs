namespace Blaxpro.Sql
{
    public interface IDbMigrator
    {
        void add<TMigration>() where TMigration : class, IMigration, new();
        IMigrationResult upgrade(IDb db);
    }
}