using System.Collections.Generic;

namespace Blaxpro.Sql.Models
{
    public interface IMigration
    {
        string Name { get; }
        IEnumerable<IMigration> getDependencies();
        IEnumerable<IQuery> getUpgrades();
        IEnumerable<IQuery> getDowngrades();
    }
}