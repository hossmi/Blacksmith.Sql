﻿using System;
using System.Collections.Generic;
using System.Linq;
using Blaxpro.Sql.Models;
using Blaxpro.Validations;

namespace Blaxpro.Sql
{
    public class DbMigrator : IDbMigrator
    {
        private readonly ISet<IMigration> migrations;
        private readonly Asserts assert;


        public DbMigrator(MigrationSettings settings)
        {
            this.assert = Asserts.Assert;

            this.assert.isNotNull(settings);
            this.assert.stringIsNotEmpty(settings.MigrationsTable);
            this.Settings = settings;
        }

        public DbMigrator() : this(DefaultSettings)
        {
            this.migrations = new HashSet<IMigration>(new PrvMigrationEqualityComparer());
        }

        public MigrationSettings Settings { get; }

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

        public void add(IMigration migration)
        {
            this.migrations.Add(migration);
        }

        public IMigrationStep getLast(IDb db)
        {
            using (ITransaction transaction = db.transact())
            {
                IMigrationStep migrationStep;
                Query query;

                query = $@"
SELECT TOP 1 [name], [date]
FROM [{this.Settings.MigrationsTable}]
ORDER BY [date] DESC;";

                migrationStep = transaction
                    .get(query)
                    .Select(r => (IMigrationStep)new PrvMigrationStep
                    {
                         Name = (string)r["name"],
                          Date = (DateTime)r["date"],
                    }).FirstOrDefault();

                throw new NotImplementedException();
            }
            throw new System.NotImplementedException();
        }

        public IEnumerable<IQuery> getQueries(string fromVersion, string toVersion)
        {
            throw new System.NotImplementedException();
        }

        public IMigrationResult upgrade(IDb db)
        {
            throw new System.NotImplementedException();
        }

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
        }
    }
}