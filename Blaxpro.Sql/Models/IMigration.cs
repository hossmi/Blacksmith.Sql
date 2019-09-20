using System.Collections.Generic;

namespace Blaxpro.Sql.Models
{
    public interface IMigration
    {
        IEnumerable<IMigration> getDependencies();
        IEnumerable<IQuery> getUpgrades();
        IEnumerable<IQuery> getDowngrades();
    }
}