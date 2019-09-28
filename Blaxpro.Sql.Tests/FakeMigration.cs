using Blaxpro.Sql.Models;
using System.Collections.Generic;
using System.Linq;

namespace Blaxpro.Sql.Tests
{
    public class FakeMigration : IMigration
    {
        public FakeMigration(string name)
        {
            this.Name = name;
            this.Upgrades = Enumerable.Empty<IQuery>();
            this.Downgrades = Enumerable.Empty<IQuery>();
            this.Dependencies = Enumerable.Empty<FakeMigration>();
        }

        public string Name { get; }
        public IEnumerable<IQuery> Upgrades { get; set; }
        public IEnumerable<IQuery> Downgrades { get; set; }
        public IEnumerable<FakeMigration> Dependencies { get; set; }

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
