using System;
using System.Collections.Generic;
using System.Linq;
using Blaxpro.Sql.Exceptions;
using Blaxpro.Sql.Models;
using Blaxpro.Sql.Tests.Fakes;
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

        [Fact]
        public void fake_migration_tests()
        {
            IDbMigrator migrator;
            IReadOnlyList<IMigrationStep> steps;
            IList<IMigrationStep> migrationHistory;

            migrator = new PrvDbMigrator();

            migrator.add(this.migrations["V2"]);

            Assert.False(migrator.isInitialized(this.db));
            Assert.Throws<MigrationsSetupException>(() => migrator.getHistory(this.db));
            Assert.Throws<MigrationsSetupException>(() => migrator.upgrade(this.db));

            migrator.initialize(this.db);

            Assert.True(migrator.isInitialized(this.db));

            steps = migrator.upgrade(this.db);
            Assert.Equal(2, steps.Count);
            Assert.Equal("V1", steps[0].Name);
            Assert.Equal(MigrationDirection.Up, steps[0].Direction);
            Assert.Equal("V2", steps[1].Name);
            Assert.Equal(MigrationDirection.Up, steps[1].Direction);

            steps = migrator.upgrade(this.db);
            Assert.Empty(steps);

            migrator.add(this.migrations["V3"]);
            Assert.Throws<DbMigrationException>(() => migrator.upgrade(this.db));

            migrationHistory = migrator
                .getHistory(this.db)
                .ToList();

            Assert.Equal(2, migrationHistory.Count);
            Assert.Equal("V1", migrationHistory[0].Name);
            Assert.Equal(MigrationDirection.Up, steps[0].Direction);
            Assert.Equal("V2", migrationHistory[1].Name);
            Assert.Equal(MigrationDirection.Up, steps[1].Direction);

            steps = migrator.remove(this.db, "V1");

            Assert.Equal(2, steps.Count);
            Assert.Equal("V2", steps[0].Name);
            Assert.Equal(MigrationDirection.Down, steps[0].Direction);
            Assert.Equal("V1", steps[1].Name);
            Assert.Equal(MigrationDirection.Down, steps[1].Direction);

            migrationHistory = migrator
                .getHistory(this.db)
                .ToList();

            Assert.Equal(4, migrationHistory.Count);

            Assert.Equal("V1", migrationHistory[0].Name);
            Assert.Equal(MigrationDirection.Up, migrationHistory[0].Direction);
            Assert.Equal("V2", migrationHistory[1].Name);
            Assert.Equal(MigrationDirection.Up, migrationHistory[1].Direction);

            Assert.Equal("V2", migrationHistory[0].Name);
            Assert.Equal(MigrationDirection.Down, migrationHistory[0].Direction);
            Assert.Equal("V1", migrationHistory[1].Name);
            Assert.Equal(MigrationDirection.Down, migrationHistory[1].Direction);
        }

        private static void prv_db_BeginTransaction(FakeTransaction transaction)
        {
            transaction.InvokedSet += prv_transaction_InvokedSet;
        }

        private static int prv_transaction_InvokedSet(IQuery query)
        {
            if (query.Statement == "BOOM")
                throw new DbCommandExecutionException(new Exception("Boooom"));

            return random.Next(1, 34);
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
                this.fakeMigrationHistory.Add(new PrvMigrationStep(name) { Direction = MigrationDirection.Up });
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
            public MigrationDirection Direction { get; set; }
        }
    }
}
