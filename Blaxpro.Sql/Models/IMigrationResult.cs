using System.Collections.Generic;

namespace Blaxpro.Sql.Models
{
    public interface IMigrationResult : IReadOnlyList<IMigrationStep>
    {
        IMigrationStep Previous { get; }
        IMigrationStep Current { get; }
    }
}