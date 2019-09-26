using System.Collections.Generic;
using System.Linq;
using Blaxpro.Sql.Exceptions;
using Blaxpro.Sql.Models;
using Xunit;

namespace Blaxpro.Sql.Tests
{
    public class MigrationTests
    {
        [Fact]
        public void sqlServer_migration_tests()
        {
            IDb db;
            IDbMigrator dbMigrator;
            IMigrableDb migrableDb;
            IMigrationStep[] steps;
            IList<IMigrationStep> migrationHistory;

            db = new Db(Connections.getSqlServerConnection);
            dbMigrator = new SqlDbMigrator();
            dbMigrator.add(new V2AddUserBirthDateColumnMigration());
            dbMigrator.add(new V1CreateUsersTableMigration());

            migrableDb = dbMigrator.getFor(db);

            Assert.False(migrableDb.IsInitialized);
            Assert.Throws<MigrationsSetupException>(() => migrableDb.History);
            Assert.Throws<MigrationsSetupException>(() => migrableDb.upgrade());

            migrableDb.initialize();

            Assert.True(migrableDb.IsInitialized);

            steps = migrableDb.upgrade();
            Assert.Equal(2, steps.Length);
            Assert.Equal(V1CreateUsersTableMigration.Create_users_table, steps[0].Name);
            Assert.Equal(V2AddUserBirthDateColumnMigration.AddUsersBirthdateColumn, steps[1].Name);

            steps = migrableDb.upgrade();
            Assert.Empty(steps);

            dbMigrator.add(new V3WrongMigration());
            migrableDb = dbMigrator.getFor(db);
            Assert.Throws<DbMigrationException>(() => migrableDb.upgrade());

            migrationHistory = migrableDb.History.ToList();
            Assert.Equal(3, steps.Length);
            Assert.Equal(SqlDbMigrator.DefaultSettings.InitialMigrationName, steps[0].Name);
            Assert.Equal(V1CreateUsersTableMigration.Create_users_table, steps[1].Name);
            Assert.Equal(V2AddUserBirthDateColumnMigration.AddUsersBirthdateColumn, steps[2].Name);
        }

        [Fact]
        public void fake_migration_tests()
        {
            IDb db;
            IDbMigrator dbMigrator;
            IMigrableDb migrableDb;
            IMigrationStep[] steps;
            IList<IMigrationStep> migrationHistory;
            IMigration[] migrations;

            migrations = new IMigration[]
            {
                new FakeMigration
                {
                    Name = "V2",
                    Dependencies = new IMigration[]
                    {
                        new FakeMigration
                        {
                            Name = "V1",
                        },
                    },
                },
                new FakeMigration
                {
                    Name = "V1",
                },
            };

            db = new FakeDb();
            dbMigrator = new FakeDbMigrator();
            dbMigrator.add(new FakeMigration("V2", new IMigration[] { new FakeMigration("V1") }));
            dbMigrator.add(new FakeMigration("V1"));

            migrableDb = dbMigrator.getFor(db);

            Assert.False(migrableDb.IsInitialized);
            Assert.Throws<MigrationsSetupException>(() => migrableDb.History);
            Assert.Throws<MigrationsSetupException>(() => migrableDb.upgrade());

            migrableDb.initialize();

            Assert.True(migrableDb.IsInitialized);

            steps = migrableDb.upgrade();
            Assert.Equal(2, steps.Length);
            Assert.Equal(V1CreateUsersTableMigration.Create_users_table, steps[0].Name);
            Assert.Equal(V2AddUserBirthDateColumnMigration.AddUsersBirthdateColumn, steps[1].Name);

            steps = migrableDb.upgrade();
            Assert.Empty(steps);

            dbMigrator.add(new V3WrongMigration());
            migrableDb = dbMigrator.getFor(db);
            Assert.Throws<DbMigrationException>(() => migrableDb.upgrade());

            migrationHistory = migrableDb.History.ToList();
            Assert.Equal(3, steps.Length);
            Assert.Equal(SqlDbMigrator.DefaultSettings.InitialMigrationName, steps[0].Name);
            Assert.Equal(V1CreateUsersTableMigration.Create_users_table, steps[1].Name);
            Assert.Equal(V2AddUserBirthDateColumnMigration.AddUsersBirthdateColumn, steps[2].Name);
        }
    }
}
