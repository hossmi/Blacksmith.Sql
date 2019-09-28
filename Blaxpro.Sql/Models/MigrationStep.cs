using System;

namespace Blaxpro.Sql.Models
{
    internal class MigrationStep : IMigrationStep
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
}
