using Blaxpro.Sql.Models;
using System.Collections.Generic;

namespace Blaxpro.Sql.Tests
{
    public class FakeMigration : IMigration
    {
        public string Name { get; set; }
        public IEnumerable<IQuery> Upgrades { get; set; }
        public IEnumerable<IQuery> Downgrades { get; set; }
        public IEnumerable<IMigration> Dependencies { get; set; }

        public IEnumerable<IMigration> getDependencies()
        {
            return this.Dependencies;
        }

        public IEnumerable<IQuery> getDowngrades()
        {
            return this.Downgrades;
        }

        public IEnumerable<IQuery> getUpgrades()
        {
            return this.Upgrades;
        }
    }
}
