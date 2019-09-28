using System;
using System.Collections.Generic;
using System.Linq;
using Blaxpro.Sql.Exceptions;
using Blaxpro.Sql.Models;
using Xunit;

namespace Blaxpro.Sql.Tests
{
    public class FakeMigrationTests
    {
        private static Random random;
        private readonly FakeDb db;
        private readonly IDictionary<string, FakeMigration> migrations;

        static FakeMigrationTests()
        {
            random = new Random((Environment.CurrentManagedThreadId * Environment.TickCount) % int.MaxValue);
        }

        public FakeMigrationTests()
        {
            this.migrations = new Dictionary<string, FakeMigration>
            {
                { "V1", new FakeMigration("V1") },
                { "V2", new FakeMigration("V2") },
                { "V3", new FakeMigration("V3") },
            };

            this.migrations["V2"].Dependencies = new[] { this.migrations["V1"] };
            this.migrations["V3"].Dependencies = new[] { this.migrations["V1"], this.migrations["V2"] };
            this.migrations["V3"].Upgrades = new IQuery[] { (Query)"BOOM" };

            this.db = new FakeDb();
            this.db.BeginTransaction += prv_db_BeginTransaction;
        }

        private void prv_db_BeginTransaction(FakeTransaction transaction)
        {
            transaction.InvokedSet += prv_transaction_InvokedSet;
        }

        private int prv_transaction_InvokedSet(IQuery query)
        {
            if (query.Statement == "BOOM")
                throw new DbCommandExecutionException(new Exception("Boooom"));

            return random.Next(1, 34);
        }

        [Fact]
        public void fake_migration_tests()
        {
            IDbMigrator dbMigrator;
            IMigrableDb migrableDb;
            IMigrationStep[] steps;
            IList<IMigrationStep> migrationHistory;

            dbMigrator = new PrvDbMigrator();

            dbMigrator.add(this.migrations["V2"]);
            migrableDb = dbMigrator.getFor(this.db);

            Assert.False(migrableDb.IsInitialized);
            Assert.Throws<MigrationsSetupException>(() => migrableDb.History);
            Assert.Throws<MigrationsSetupException>(() => migrableDb.upgrade());

            migrableDb.initialize();

            Assert.True(migrableDb.IsInitialized);

            steps = migrableDb.upgrade();
            Assert.Equal(2, steps.Length);
            Assert.Equal("V1", steps[0].Name);
            Assert.Equal("V2", steps[1].Name);

            steps = migrableDb.upgrade();
            Assert.Empty(steps);

            dbMigrator.add(this.migrations["V3"]);
            migrableDb = dbMigrator.getFor(this.db);
            Assert.Throws<DbMigrationException>(migrableDb.upgrade);

            migrationHistory = migrableDb.History.ToList();
            Assert.Equal(2, migrationHistory.Count);
            Assert.Equal("V1", migrationHistory[0].Name);
            Assert.Equal("V2", migrationHistory[1].Name);
        }

        private class PrvDbMigrator : AbstractDbMigrator
        {
            private readonly ICollection<IMigrationStep> fakeMigrationHistory;
            private bool existsMigrationsTable;

            public PrvDbMigrator()
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
        }

        private class PrvMigrationStep : IMigrationStep
        {
            public PrvMigrationStep(string name)
            {
                this.Name = name;
                this.Date = DateTime.UtcNow;
            }

            public string Name { get; set; }
            public DateTime Date { get; set; }
        }
    }
}
