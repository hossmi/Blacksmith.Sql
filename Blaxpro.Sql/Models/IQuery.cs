using System.Collections.Generic;

namespace Blaxpro.Sql.Models
{
    public interface IQuery
    {
        string Statement { get; }
        IEnumerable<KeyValuePair<string, object>> Parameters { get; }
    }
}