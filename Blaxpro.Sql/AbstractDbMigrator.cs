using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blaxpro.Sql.Models;
using Blaxpro.Validations;

namespace Blaxpro.Sql
{
    public class SqlDbMigrator : AbstractDbMigrator
    {
        private readonly MigrationSettings settings;

        public SqlDbMigrator(MigrationSettings settings)
        {
            this.assert.isNotNull(settings);
            this.assert.stringIsNotEmpty(settings.MigrationsTable);
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
                    InitialMigrationName = "Data base migrations initialization.",
                    Schema = "dbo",
                };
            }
        }

        protected override IMigration prv_getInitialMigration()
        {
            return new PrvInitialMigration
            {
                Name = this.settings.InitialMigrationName,
                Table = this.settings.MigrationsTable,
                Schema = this.settings.Schema,
            };
        }

        private class PrvInitialMigration : IMigration
        {

            public string Name { get; set; }
            public object Table { get; set; }
            public string Schema { get; set; }

            public IEnumerable<IMigration> getDependencies()
            {
                yield break;
            }

            public IEnumerable<IQuery> getDowngrades()
            {
                yield return (Query)$@"DROP TABLE [{this.Schema}].[{this.Table}];";
            }

            public IEnumerable<IQuery> getUpgrades()
            {
                yield return (Query)$@"
CREATE TABLE [{this.Schema}].[{this.Table}]
(
    id INTEGER PRIMARY KEY,
    [name] NVARCHAR(1024) NOT NULL,
    [date] DATETIME NOT NULL,
    [action] NVARCHAR(32) NOT NULL
);";
            }
        }
    }

    public abstract class AbstractDbMigrator : IDbMigrator
    {
        protected readonly Asserts assert;
        private readonly IDictionary<string, IMigration> migrations;

        public AbstractDbMigrator()
        {
            IMigration initialMigration;

            this.assert = Asserts.Assert;

            initialMigration = prv_getInitialMigration();
            this.assert.isNotNull(initialMigration);

            this.migrations = new Dictionary<string, IMigration>();
            this.migrations.Add(initialMigration.Name, initialMigration);
        }

        protected abstract IMigration prv_getInitialMigration();

        public IMigrationReport getCurrentState(IDb db)
        {
            IList<IMigrationStep> migrationSteps;

            migrationSteps = prv_getMigrations(db).ToList();

            if(migrationSteps.Count == 0)
            {
                migrationSteps = prv_getMigrationsRecursively(this.migrations.Values)
                    .Select(m => new PrvMigrationStep
                    {
                         Name = m.Name,
                          Date = null,
                    })
                    .Cast<IMigrationStep>()
                    .ToList();

                return new PrvMigrationReport(migrationSteps);
            }
            else
            {

            }

            prv_getCurrentState(this.migrations, db, migrationSteps);

            return new PrvMigrationReport(migrationSteps);
        }

        private IEnumerable<IMigration> prv_getMigrationsRecursively(IEnumerable<IMigration> migrations)
        {
            throw new NotImplementedException();
        }

        internal abstract IEnumerable<IMigrationStep> prv_getMigrations(IDb db);

        private static void prv_getCurrentState(
            IDictionary<string, IMigration> migrations
            , IDb db
            , IList<IMigrationStep> migrationSteps)
        {



            throw new NotImplementedException();
        }

        public IMigrationReport upgrade(IDb db)
        {
            throw new NotImplementedException();
        }

        public IMigrationReport downgradeTo(string migrationName, IDb db)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IQuery> getQueries(string fromMigration, string toMigration)
        {
            throw new NotImplementedException();
        }

        public void add(IMigration migration)
        {
            this.assert.isNotNull(migration);
            this.assert.isFalse(this.migrations.ContainsKey(migration.Name), $"The migration '{migration.Name}' has already added.");

            this.migrations.Add(migration.Name, migration);
        }

        //        public IMigrationStep getEnabledMigrations(IDb db)
        //        {
        //            using (ITransaction transaction = db.transact())
        //            {
        //                IMigrationStep migrationStep;
        //                Query query;

        //                query = $@"
        //SELECT TOP 1 [name], [date]
        //FROM [{this.Settings.MigrationsTable}]
        //ORDER BY [date] DESC;";

        //                migrationStep = transaction
        //                    .get(query)
        //                    .Select(r => (IMigrationStep)new PrvMigrationStep
        //                    {
        //                         Name = (string)r["name"],
        //                          Date = (DateTime)r["date"],
        //                    }).FirstOrDefault();

        //                throw new NotImplementedException();
        //            }
        //            throw new System.NotImplementedException();
        //        }

        private class PrvMigrationStep : IMigrationStep
        {
            public string Name { get; set; }
            public DateTime? Date { get; set; }
        }

        private class PrvMigrationReport : IMigrationReport
        {
            private readonly IList<IMigrationStep> migrations;

            public PrvMigrationReport(IList<IMigrationStep> migrationSteps)
            {
                this.migrations = migrationSteps;
            }

            public IMigrationStep this[int index] => this.migrations[index];

            public int Count => this.migrations.Count;

            public IEnumerator<IMigrationStep> GetEnumerator()
            {
                return this.migrations.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.migrations.GetEnumerator();
            }
        }
    }
}