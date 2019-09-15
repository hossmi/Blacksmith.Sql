using Blaxpro.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blaxpro.Sql.Models
{
    public abstract class AbstractMigration : IMigration
    {
        private readonly Asserts asserts;

        public AbstractMigration()
        {
            this.asserts = Asserts.Assert;
        }

        public string Name
        {
            get
            {
                string name;

                name = this.prv_getName();
                this.asserts.stringIsNotEmpty(name, "Migration name cannot be null or empty.");

                return name;
            }
        }

        public IEnumerable<IMigration> getDependencies()
        {
            IEnumerable<IMigration> migrations;

            migrations = prv_getDependencies();
            this.asserts.isNotNull(migrations, "Migration dependencies collection cannot be null.");

            return migrations;
        }

        public IEnumerable<IQuery> getDowngrades()
        {
            IEnumerable<IQuery> queries;

            queries = prv_getDowngrades();
            this.asserts.isNotNull(queries, "Downgrade queries collection cannot be null.");

            return queries;
        }

        public IEnumerable<IQuery> getUpgrades()
        {
            IEnumerable<IQuery> queries;

            queries = prv_getUpgrades();
            this.asserts.isNotNull(queries, "Upgrade queries collection cannot be null.");

            return queries;
        }

        protected abstract IEnumerable<IQuery> prv_getUpgrades();

        protected virtual IEnumerable<IQuery> prv_getDowngrades()
        {
            yield break;
        }

        protected virtual IEnumerable<IMigration> prv_getDependencies()
        {
            yield break;
        }

        protected abstract string prv_getName();
    }
}
