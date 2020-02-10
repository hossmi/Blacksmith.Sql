using System.Collections.Generic;
using Blacksmith.Sql.Queries;
using Blacksmith.Sql.Queries.Extensions;
using Blacksmith.Sql.Queries.MsSql;
using Blacksmith.Sql.Models;

namespace Blacksmith.Sql
{
    public class SqlDbMigrator : AbstractDbMigrator
    {
        private readonly MigrationSettings settings;

        public SqlDbMigrator(MigrationSettings settings)
        {
            this.assert.isNotNull(settings);
            this.assert.isFilled(settings.MigrationsTable);
            this.settings = settings;
        }

        public SqlDbMigrator() : this(DefaultSettings)
        {
        }

        public static MigrationSettings DefaultSettings
        {
            get
            {
                return new MigrationSettings
                {
                    MigrationsTable = "__migrations",
                    Schema = "dbo",
                };
            }
        }

        protected override void prv_createMigrationsTable(ITransaction transaction)
        {
            ISqlStatement query = new SqlStatement($@"
CREATE TABLE [{this.settings.Schema}].[{this.settings.MigrationsTable}]
(
    id INTEGER PRIMARY KEY,
    [name] NVARCHAR(1024) NOT NULL,
    [date] DATETIME NOT NULL,
    [action] NVARCHAR(32) NOT NULL
);
GO");

            transaction.set(query);
        }

        protected override IEnumerable<IMigrationStep> prv_getMigrationHistory(ITransaction transaction)
        {
            throw new System.NotImplementedException();
        }

        protected override bool prv_existsMigrationsTable(ITransaction transaction)
        {
            throw new System.NotImplementedException();
        }

        protected override void prv_insertMigration(ITransaction transaction, string name)
        {
            throw new System.NotImplementedException();
        }
    }
}