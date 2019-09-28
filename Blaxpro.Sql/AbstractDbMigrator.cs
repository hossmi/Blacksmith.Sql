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
            , IEnumerable<IMigration> migrations
            , ref IDictionary<string, IMigrationStep> migrationHistory)
        {
            IEnumerable<IMigrationStep> executedSteps;

            executedSteps = Enumerable.Empty<IMigrationStep>();

            foreach (IMigration migration in migrations)
            {
                IEnumerable<IMigrationStep> currentMigrationExecutedSteps;
                MigrationStep currentStep;

                if (migrationHistory.ContainsKey(migration.Name))
                    continue;

                currentMigrationExecutedSteps = prv_upgrade(transaction, migration.getDependencies(), ref migrationHistory);

                foreach (IQuery query in migration.getUpgrades())
                    transaction.set(query);

                this.prv_insertMigration(transaction, migration.Name);

                currentStep = new MigrationStep
                {
                    Name = migration.Name,
                    Date = DateTime.UtcNow,
                    Direction = MigrationDirection.Up,
                };

                migrationHistory.Add(currentStep.Name, currentStep);

                executedSteps = executedSteps
                    .Concat(currentMigrationExecutedSteps)
                    .Concat(new[] { currentStep });
            }

            return executedSteps;
        }

        private static IDictionary<string, IMigration> prv_getAllMigrations(IEnumerable<IMigration> migrations)
        {
            IDictionary<string, IMigration> allMigrations;

            allMigrations = new Dictionary<string, IMigration>();
            prv_getAllMigrations(migrations, ref allMigrations);

            return allMigrations;
        }

        private static void prv_getAllMigrations(IEnumerable<IMigration> migrations, ref IDictionary<string, IMigration> allMigrations)
        {
            foreach (IMigration migration in migrations)
            {
                if (false == allMigrations.ContainsKey(migration.Name))
                {
                    prv_getAllMigrations(migration.getDependencies(), ref allMigrations);
                    allMigrations.Add(migration.Name, migration);
                }
            }
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

                try
                {
                    using (ITransaction transaction = this.Db.transact())
                    {
                        IDictionary<string, IMigrationStep> migrationHistory;

                        this.migrator.prv_assertIsInitialized(transaction);

                        migrationHistory = this.migrator
                                .prv_getMigrationHistory(transaction)
                                .Select(step => prv_assertStepHasMatchingMigration(step, migrations))
                                .ToDictionary(step => step.Name);

                        executedSteps = this.migrator
                            .prv_upgrade(transaction, this.migrator.migrations.Values, ref migrationHistory)
                            .ToArray();

                        transaction.saveChanges();
                    }
                }
                catch (DbCommandExecutionException ex)
                {
                    throw new DbMigrationException("Error upgrading database", ex);
                }

                return executedSteps;
            }

            public IMigrationStep[] downgrade(string migrationName)
            {
                throw new NotImplementedException();
            }

        }
    }
}
