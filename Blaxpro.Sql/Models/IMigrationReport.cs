using System.Collections.Generic;

namespace Blaxpro.Sql.Models
{
    public interface IMigrationReport : IReadOnlyList<IMigrationStep>
    {
    }
}