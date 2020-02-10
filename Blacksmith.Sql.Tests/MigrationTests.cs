using System.Collections.Generic;
using System.Linq;
using Blacksmith.Sql.Exceptions;
using Blacksmith.Sql.Models;
using Xunit;

namespace Blacksmith.Sql.Tests
{
    public class MigrationTests
    {
        [Fact]
        public void sqlServer_migration_tests()
        {
            IDb db;
            IDbMigrator migrator;
            IReadOnlyList<IMigrationStep> steps;
            IList<IMigrationStep> migrationHistory;

            db = new Db(Connections.getSqlServerConnection);
            migrator = new SqlDbMigrator();
            migrator.add(new V2AddUserBirthDateColumnMigration());
            migrator.add(new V1CreateUsersTableMigration());

            Assert.False(migrator.isInitialized(db));
            Assert.Throws<MigrationsSetupException>(() => migrator.getHistory(db));
            Assert.Throws<MigrationsSetupException>(() => migrator.upgrade(db));

            migrator.initialize(db);

            Assert.True(migrator.isInitialized(db));

            steps = migrator.upgrade(db);
            Assert.Equal(2, steps.Count);
            Assert.Equal(V1CreateUsersTableMigration.Create_users_table, steps[0].Name);
            Assert.Equal(V2AddUserBirthDateColumnMigration.AddUsersBirthdateColumn, steps[1].Name);

            steps = migrator.upgrade(db);
            Assert.Empty(steps);

            migrator.add(new V3WrongMigration());
            Assert.Throws<DbMigrationException>(() => migrator.upgrade(db));

            migrationHistory = migrator
                .getHistory(db)
                .ToList();

            Assert.Equal(2, steps.Count);
            Assert.Equal(V1CreateUsersTableMigration.Create_users_table, steps[1].Name);
            Assert.Equal(V2AddUserBirthDateColumnMigration.AddUsersBirthdateColumn, steps[2].Name);
        }
    }
}
