using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blaxpro.Sql.Exceptions;
using Blaxpro.Sql.Models;
using Blaxpro.Validations;

namespace Blaxpro.Sql
{

    public abstract class AbstractDbMigrator : IDbMigrator
    {
        protected readonly Asserts assert;
        private readonly IDictionary<string, IMigration> migrations;

        public AbstractDbMigrator()
        {
            this.assert = Asserts.Assert;

            this.migrations = new Dictionary<string, IMigration>();
        }

        public void add(IMigration migration)
        {
            this.assert.isNotNull(migration);
            this.assert.isFalse(this.migrations.ContainsKey(migration.Name), $"The migration '{migration.Name}' has already added.");

            this.migrations.Add(migration.Name, migration);
        }

        public IMigrableDb getFor(IDb db)
        {
            return new PrvMigrableDb(this, db);
        }

        private class PrvMigrableDb : IMigrableDb
        {
            private readonly AbstractDbMigrator migrator;

            public PrvMigrableDb(AbstractDbMigrator migrator, IDb db)
            {
                this.migrator = migrator;
                this.Db = db;
            }

            public IDb Db { get; }
            public IEnumerable<IMigrationStep> History
            {
                get
                {
                    if (this.IsInitialized == false)
                        throw new UninitializedMigrationsException($"Database migrations system has not been initialized. Call '{nameof(this.initialize)}' method first.");

                    using (ITransaction transaction = this.Db.transact())
                        return this.migrator.prv_getMigrationHistory(transaction);
                }
            }

            public bool IsInitialized
            {
                get
                {
                    using (ITransaction transaction = this.Db.transact())
                        return this.migrator.prv_existsMigrationsTable(transaction);
                }
            }

            public IMigrationStep[] downgradeTo(string migrationName)
            {
                throw new NotImplementedException();
            }

            public void initialize()
            {
                if (this.IsInitialized)
                    throw new AlreadyInitializedMigrationsException($"Database migrations system has already been initialized.");

                using (ITransaction transaction = this.Db.transact())
                {
                    IMigrationStep[] history;
                    bool migrationsTableExists;

                    this.migrator.prv_createMigrationsTable(transaction);
                    migrationsTableExists = this.migrator.prv_existsMigrationsTable(transaction);

                    if(migrationsTableExists == false)
                        throw new DbMigrationException($"Migrations table could not been created.");

                    history = this.migrator
                        .prv_getMigrationHistory(transaction)
                        .ToArray();

                    if (history.Length != 0)
                        throw new DbMigrationException($"Just created migrations table cannot have registers.");

                    transaction.saveChanges();
                }
            }

            public IMigrationStep[] upgrade()
            {
                throw new NotImplementedException();
            }
        }

        protected abstract void prv_createMigrationsTable(ITransaction transaction);
        protected abstract IEnumerable<IMigrationStep> prv_getMigrationHistory(ITransaction transaction);
        protected abstract bool prv_existsMigrationsTable(ITransaction transaction);

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

    }
}