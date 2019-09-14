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
            dbMigrator.add<V1StartMigration>();
            dbMigrator.add<V2AddUserBirthDateColumnMigration>();

            migrationResult = dbMigrator.upgrade(db);
            Assert.Equal(2, migrationResult.Count);
            Assert.Equal(0, migrationResult.PreviousVersion);
            Assert.Equal(2, migrationResult.CurrentVersion);

            migrationResult = dbMigrator.upgrade(db);
            Assert.Equal(0, migrationResult.Count);
            Assert.Equal(2, migrationResult.PreviousVersion);
            Assert.Equal(2, migrationResult.CurrentVersion);

            dbMigrator.add<V3WrongMigration>();

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
    }
}
