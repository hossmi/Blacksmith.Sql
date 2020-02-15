using System;

namespace Blacksmith.Sql.Models
{
    internal class MigrationStep : IMigrationStep
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public MigrationDirection Direction { get; set; }
    }
}
