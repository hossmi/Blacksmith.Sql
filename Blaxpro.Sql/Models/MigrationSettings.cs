namespace Blaxpro.Sql.Models
{
    public class MigrationSettings
    {
        public string MigrationsTable { get; set; }
        public string InitialMigrationName { get; set; }
        public string Schema { get; set; }
    }
}