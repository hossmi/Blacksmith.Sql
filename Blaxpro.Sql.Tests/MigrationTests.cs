using Blaxpro.Sql.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Blaxpro.Sql.Tests
{
    public class MigrationTests
    {
        [Fact]
        public void sqlServer_tests()
        {
            IDb db;
            IDbMigrator dbMigrator;
            IMigrationResult migrationResult;
            IMigrationStep currentStep;

            db = new Db(Connections.getSqlServerConnection);
            dbMigrator = new DbMigrator();

            dbMigrator.add(new V2AddUserBirthDateColumnMigration());
            migrationResult = dbMigrator.upgrade(db);

            Assert.Equal(2, migrationResult.Count);
            Assert.Null(migrationResult.Previous);
            Assert.Equal(V2AddUserBirthDateColumnMigration.AddUsersBirthdateColumn, migrationResult.Current.Name);

            migrationResult = dbMigrator.upgrade(db);
            Assert.Equal(0, migrationResult.Count);
            Assert.Equal(V2AddUserBirthDateColumnMigration.AddUsersBirthdateColumn, migrationResult.Previous.Name);
            Assert.Equal(V2AddUserBirthDateColumnMigration.AddUsersBirthdateColumn, migrationResult.Current.Name);

            dbMigrator.add(new V3WrongMigration());

            Assert.Throws<DbMigrationException>(() => dbMigrator.upgrade(db));

            currentStep = dbMigrator.getLast(db);
            Assert.Equal(V2AddUserBirthDateColumnMigration.AddUsersBirthdateColumn, currentStep.Name);
        }

        [Fact]
        public void sqlite_tests()
        {
            IDb db;

            db = new Db(Connections.getSqliteConnection);

            throw new NotImplementedException();
        }

        [Fact]
        public void get_migration_scripts_tests()
        {
            IDbMigrator dbMigrator;
            IList<IQuery> migrationQueries;

            dbMigrator = new DbMigrator();
            dbMigrator.add(new V3WrongMigration());

            migrationQueries = dbMigrator
                .getQueries(V1CreateUsersTableMigration.Create_users_table, V3WrongMigration.Boooooom)
                .ToList();

            Assert.Equal(2, migrationQueries.Count);
            


        }
    }
}
