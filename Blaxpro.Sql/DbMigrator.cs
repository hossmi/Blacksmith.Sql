using System.Collections.Generic;

namespace Blaxpro.Sql
{
    public class DbMigrator : IDbMigrator
    {
        private readonly IList<IMigration> migrations;

        public DbMigrator()
        {
            this.migrations = new List<IMigration>();
        }

        public void add(IMigration migration)
        {
            this.migrations.Add(migration);
        }

        public IEnumerable<IMigrationScript> getScripts(int fromVersion, int toVersion)
        {
            throw new System.NotImplementedException();
        }

        public IMigrationResult upgrade(IDb db)
        {
            PrvMigrationStep migrationStep;

            migrationStep = prv_getMigrationsSequence(this.migrations);


            throw new System.NotImplementedException();
        }

        private class PrvMigrationStep
        {
        }
    }
}