using System.Collections.Generic;

namespace Blaxpro.Sql.Models
{
    public interface IQuery
    {
        string Statement { get; }
        IDictionary<string, object> Parameters { get; }
        int write();
        IEnumerable<T> read<T>() where T : class, new();
        object readScalar();
    }
}