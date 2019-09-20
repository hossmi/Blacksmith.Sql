using System;
using System.Collections.Generic;
using System.Linq;
using Blaxpro.Sql.Models;
using Blaxpro.Validations;

namespace Blaxpro.Sql
{
    public class DbMigrator : IDbMigrator
    {
        private readonly Asserts assert;
        private readonly IDictionary<string, IMigration> migrations;
        private readonly MigrationSettings settings;

        public DbMigrator(MigrationSettings settings)
        {
            this.assert = Asserts.Assert;

            this.assert.isNotNull(settings);
            this.assert.stringIsNotEmpty(settings.MigrationsTable);

            this.settings = settings;
            this.migrations = new Dictionary<string, IMigration>();
        }

        public DbMigrator() : this(DefaultSettings)
        {
        }

        public static MigrationSettings DefaultSettings
        {
            get
            {
                return new MigrationSettings
                {
                    MigrationsTable = "__migrations",
                };
            }
        }

        public IMigrationReport getCurrentDbState(IDb db)
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

        public void set(IMigration migration)
        {
            throw new NotImplementedException();
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

        private class PrvMigrationEqualityComparer : IEqualityComparer<IMigration>
        {
            public bool Equals(IMigration x, IMigration y)
            {
                return x.Name.Equals(y.Name);
            }

            public int GetHashCode(IMigration obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        private class PrvMigrationStep : IMigrationStep
        {
            public string Name { get; set; }
            public DateTime Date { get; set; }
            public bool Enabled { get; set; }
        }
    }
}