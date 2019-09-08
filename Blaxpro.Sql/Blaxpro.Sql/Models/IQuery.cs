using System.Collections.Generic;

namespace Blaxpro.Sql.Models
{
    public interface IQuery
    {
        IQuery setParameter(string name, object value);
        int write();
        IEnumerable<T> read<T>() where T : class, new();
    }
}