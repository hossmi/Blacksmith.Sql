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

            db = new Db(Connections.getSqlServerConnection);
            dbMigrator = new DbMigrator();
            dbMigrator.add(new V1CreateUsersTableMigration());
            dbMigrator.add(new V2AddUserBirthDateColumnMigration());

            migrationResult = dbMigrator.upgrade(db);
            Assert.Equal(2, migrationResult.Count);
            Assert.Equal(0, migrationResult.PreviousVersion);
            Assert.Equal(2, migrationResult.CurrentVersion);

            migrationResult = dbMigrator.upgrade(db);
            Assert.Equal(0, migrationResult.Count);
            Assert.Equal(2, migrationResult.PreviousVersion);
            Assert.Equal(2, migrationResult.CurrentVersion);

            dbMigrator.add(new V3WrongMigration());

            Assert.Throws<DbMigrationException>(() =>
            {
                migrationResult = dbMigrator.upgrade(db);
            });

            Assert.Equal(2, migrationResult.CurrentVersion);

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
            IList<IMigrationScript> migrationScripts;

            dbMigrator = new DbMigrator();
            dbMigrator.add(new V1CreateUsersTableMigration());
            dbMigrator.add(new V2AddUserBirthDateColumnMigration());
            dbMigrator.add(new V3WrongMigration());

            migrationScripts = dbMigrator
                .getScripts(fromVersion: 1, toVersion: 3)
                .ToList();

            Assert.Equal(3, migrationScripts.Count);
            


        }
    }
}
