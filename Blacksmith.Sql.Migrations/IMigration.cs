using Blacksmith.Sql.Queries;
using System.Collections.Generic;

namespace Blacksmith.Sql.Models
{
    public interface IMigration 
    {
        string Name { get; }
        IEnumerable<IMigration> getDependencies();
        IEnumerable<ISqlStatement> getUpgrades();
        IEnumerable<ISqlStatement> getDowngrades();
    }
}