using System;

namespace Blaxpro.Sql.Models
{
    public interface IMigrationStep
    {
        string Name { get; }
        DateTime Date { get; }
        MigrationDirection Direction { get; }
    }
}