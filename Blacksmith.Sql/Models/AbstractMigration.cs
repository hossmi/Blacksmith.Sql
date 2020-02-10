using Blacksmith.Sql.Queries;
using Blacksmith.Validations;
using System.Collections.Generic;

namespace Blacksmith.Sql.Models
{
    public abstract class AbstractMigration : IMigration
    {
        private readonly IValidator asserts;

        public AbstractMigration()
        {
            this.asserts = Asserts.Default;
        }

        public string Name
        {
            get
            {
                string name = prv_getName();
                this.asserts.isFilled(name);

                return name;
            }
        }

        public IEnumerable<IMigration> getDependencies()
        {
            IEnumerable<IMigration> migrations;

            migrations = prv_getDependencies();
            this.asserts.isNotNull(migrations);

            return migrations;
        }

        public IEnumerable<IQuery> getDowngrades()
        {
            IEnumerable<IQuery> queries;

            queries = prv_getDowngrades();
            this.asserts.isNotNull(queries);

            return queries;
        }

        public IEnumerable<IQuery> getUpgrades()
        {
            IEnumerable<IQuery> queries;

            queries = prv_getUpgrades();
            this.asserts.isNotNull(queries);

            return queries;
        }

        protected abstract IEnumerable<IQuery> prv_getUpgrades();

        protected abstract string prv_getName();

        protected virtual IEnumerable<IQuery> prv_getDowngrades()
        {
            yield break;
        }

        protected virtual IEnumerable<IMigration> prv_getDependencies()
        {
            yield break;
        }
    }
}
