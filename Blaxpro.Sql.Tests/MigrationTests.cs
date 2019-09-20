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
            IMigrationReport migrationReport;

            db = new Db(Connections.getSqlServerConnection);
            dbMigrator = new DbMigrator();
            dbMigrator.set(new V2AddUserBirthDateColumnMigration());
            dbMigrator.set(new V1CreateUsersTableMigration());

            migrationReport = dbMigrator.upgrade(db);

            Assert.Equal(2, migrationReport.Count);
            Assert.Equal(V1CreateUsersTableMigration.Create_users_table, migrationReport[0].Name);
            Assert.Equal(V2AddUserBirthDateColumnMigration.AddUsersBirthdateColumn, migrationReport[1].Name);

            migrationReport = dbMigrator.upgrade(db);
            Assert.Equal(0, migrationReport.Count);

            dbMigrator.set(new V3WrongMigration());
            Assert.Throws<DbMigrationException>(() => dbMigrator.upgrade(db));

            migrationReport = dbMigrator.getCurrentDbState(db);
            Assert.Equal(2, migrationReport.Count);
            Assert.Equal(V1CreateUsersTableMigration.Create_users_table, migrationReport[0].Name);
            Assert.Equal(V2AddUserBirthDateColumnMigration.AddUsersBirthdateColumn, migrationReport[1].Name);
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

            migrationQueries = dbMigrator
                .getQueries(V1CreateUsersTableMigration.Create_users_table, V3WrongMigration.WrongMigration)
                .ToList();

            Assert.Equal(2, migrationQueries.Count);
        }
    }
}
