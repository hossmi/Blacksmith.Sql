using Blacksmith.Sql.Models;
using Blacksmith.Sql.Queries;
using System.Collections.Generic;
using System.Linq;

namespace Blacksmith.Sql.Tests.Fakes
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
        public IEnumerable<ISqlStatement> Upgrades { get; set; }
        public IEnumerable<ISqlStatement> Downgrades { get; set; }
        public IEnumerable<FakeMigration> Dependencies { get; set; }

        public IEnumerable<IMigration> getDependencies()
        {
            return this.Dependencies;
        }

        public IEnumerable<ISqlStatement> getDowngrades()
        {
            return this.Downgrades;
        }

        public IEnumerable<ISqlStatement> getUpgrades()
        {
            return this.Upgrades;
        }
    }
}
