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

        public IDictionary<string, IMigration> Migrations => prv_getAllMigrations(this.migrations.Values);

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

        protected abstract void prv_createMigrationsTable(ITransaction transaction);
        protected abstract IEnumerable<IMigrationStep> prv_getMigrationHistory(ITransaction transaction);
        protected abstract bool prv_existsMigrationsTable(ITransaction transaction);

        private void prv_assertIsInitialized(ITransaction transaction)
        {
            bool migrationsTableExists;

            migrationsTableExists = this.prv_existsMigrationsTable(transaction);

            if (migrationsTableExists == false)
                throw new MigrationsSetupException($"Database migrations system has not been initialized.");
        }

        private static IDictionary<string, IMigration> prv_getAllMigrations(IEnumerable<IMigration> migrations)
        {

            throw new NotImplementedException();
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
                    using (ITransaction transaction = this.Db.transact())
                    {
                        IDictionary<string, IMigration> migrations;

                        this.migrator.prv_assertIsInitialized(transaction);
                        migrations = prv_getAllMigrations(this.migrator.migrations.Values);

                        foreach (IMigrationStep step in this.migrator.prv_getMigrationHistory(transaction))
                        {
                            if (migrations.ContainsKey(step.Name))
                                yield return step;
                            else
                                throw new MigrationsSetupException($@"Missing migration instance for '{step.Name}'.");
                        }
                    }
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
                using (ITransaction transaction = this.Db.transact())
                {
                    IMigrationStep[] history;
                    bool migrationsTableExists;

                    migrationsTableExists = this.migrator.prv_existsMigrationsTable(transaction);

                    if (migrationsTableExists)
                        throw new MigrationsSetupException($"Database migrations system has already been initialized.");

                    this.migrator.prv_createMigrationsTable(transaction);
                    migrationsTableExists = this.migrator.prv_existsMigrationsTable(transaction);

                    if (migrationsTableExists == false)
                        throw new MigrationsSetupException($"Migrations table could not been created.");

                    history = this.migrator
                        .prv_getMigrationHistory(transaction)
                        .ToArray();

                    if (history.Length != 0)
                        throw new MigrationsSetupException($"New migrations table is not empty!");

                    transaction.saveChanges();
                }
            }

            public IMigrationStep[] upgrade()
            {
                using (ITransaction transaction = this.Db.transact())
                {
                    IDictionary<string, IMigration> migrations;

                    this.migrator.prv_assertIsInitialized(transaction);
                    migrations = prv_getAllMigrations(this.migrator.migrations.Values);


                }
                throw new NotImplementedException();
            }

        }
    }
}