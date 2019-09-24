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
        protected abstract void prv_insertMigration(ITransaction transaction, string name);

        private void prv_assertIsInitialized(ITransaction transaction)
        {
            bool migrationsTableExists;

            migrationsTableExists = this.prv_existsMigrationsTable(transaction);

            if (migrationsTableExists == false)
                throw new MigrationsSetupException($"Database migrations system has not been initialized.");
        }

        private IEnumerable<IMigrationStep> prv_upgrade(
            ITransaction transaction
            , IEnumerator<IMigration> iterator
            , ref IDictionary<string, IMigrationStep> migrationHistory)
        {
            if (iterator.MoveNext() == false)
                return Enumerable.Empty<IMigrationStep>();

            IMigration currentMigration;
            IEnumerator<IMigration> dependantMigrationsIterator;
            IEnumerable<IMigrationStep> executedSteps;
            MigrationStep currentStep;

            currentMigration = iterator.Current;

            if (migrationHistory.ContainsKey(currentMigration.Name))
                return prv_upgrade(transaction, iterator, ref migrationHistory);

            dependantMigrationsIterator = currentMigration
                .getDependencies()
                .GetEnumerator();

            executedSteps = prv_upgrade(transaction, dependantMigrationsIterator, ref migrationHistory);

            foreach (IQuery query in currentMigration.getUpgrades())
                transaction.set(query);

            this.prv_insertMigration(transaction, currentMigration.Name);

            currentStep = new MigrationStep
            {
                Name = currentMigration.Name,
                Date = DateTime.UtcNow,
            };

            migrationHistory.Add(currentStep.Name, currentStep);

            return executedSteps
                .Concat(new[] { currentStep });
        }

        private static IDictionary<string, IMigration> prv_getAllMigrations(IEnumerable<IMigration> migrations)
        {

            throw new NotImplementedException();
        }

        private static IMigrationStep prv_assertStepHasMatchingMigration(IMigrationStep step, IDictionary<string, IMigration> migrations)
        {
            if (migrations.ContainsKey(step.Name))
                return step;
            else
                throw new MigrationsSetupException($@"Missing migration instance for '{step.Name}'.");
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
                    IDictionary<string, IMigration> migrations;

                    migrations = prv_getAllMigrations(this.migrator.migrations.Values);

                    using (ITransaction transaction = this.Db.transact())
                    {
                        this.migrator.prv_assertIsInitialized(transaction);

                        return this.migrator
                            .prv_getMigrationHistory(transaction)
                            .Select(step => prv_assertStepHasMatchingMigration(step, migrations))
                            .ToList();
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
                IMigrationStep[] executedSteps;
                IDictionary<string, IMigration> migrations;

                executedSteps = new IMigrationStep[0];
                migrations = prv_getAllMigrations(this.migrator.migrations.Values);

                using (ITransaction transaction = this.Db.transact())
                {
                    IDictionary<string, IMigrationStep> migrationHistory;
                    IEnumerator<IMigration> migrationIterator;

                    this.migrator.prv_assertIsInitialized(transaction);

                    migrationHistory = this.migrator
                            .prv_getMigrationHistory(transaction)
                            .Select(step => prv_assertStepHasMatchingMigration(step, migrations))
                            .ToDictionary(step => step.Name);

                    migrationIterator = this.migrator
                        .migrations
                        .Values
                        .GetEnumerator();

                    executedSteps = this.migrator.prv_upgrade(transaction, migrationIterator, ref migrationHistory)
                        .ToArray();

                    transaction.saveChanges();
                }

                return executedSteps;
            }

            public IMigrationStep[] downgradeTo(string migrationName)
            {
                throw new NotImplementedException();
            }

        }
    }
}