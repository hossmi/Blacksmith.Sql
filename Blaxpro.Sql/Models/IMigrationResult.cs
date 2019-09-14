using System.Collections.Generic;

namespace Blaxpro.Sql
{
    public interface IMigrationResult : IReadOnlyList<IMigrationStepResult>
    {
        int PreviousVersion { get; }
        int CurrentVersion { get; }
    }
}