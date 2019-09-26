using System;
using System.Collections.Generic;
using Blaxpro.Sql.Models;

namespace Blaxpro.Sql.Tests
{
    public class FakeDbMigrator : AbstractDbMigrator
    {
        private readonly ICollection<IMigrationStep> fakeMigrationHistory;
        private bool existsMigrationsTable;

        public FakeDbMigrator()
        {
            this.existsMigrationsTable = false;
            this.fakeMigrationHistory = new List<IMigrationStep>();
        }
        protected override void prv_createMigrationsTable(ITransaction transaction)
        {
            this.existsMigrationsTable = true;
        }

        protected override bool prv_existsMigrationsTable(ITransaction transaction)
        {
            return this.existsMigrationsTable;
        }

        protected override IEnumerable<IMigrationStep> prv_getMigrationHistory(ITransaction transaction)
        {
            return this.fakeMigrationHistory;
        }

        protected override void prv_insertMigration(ITransaction transaction, string name)
        {
            this.fakeMigrationHistory.Add(new PrvMigrationStep(name));
        }

        private class PrvMigrationStep : IMigrationStep
        {
            public PrvMigrationStep(string name)
            {
                this.Name = name;
                this.Date = DateTime.UtcNow;
            }

            public string Name { get; set; }
            public DateTime? Date { get; set; }
        }
    }
}
