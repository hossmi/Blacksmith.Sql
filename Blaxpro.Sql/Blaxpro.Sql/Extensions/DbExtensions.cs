using Blaxpro.Sql.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blaxpro.Sql.Extensions
{
    public static class DbExtensions
    {
        public static IQuery setParameters(this IQuery query, object parameters)
        {
            throw new NotImplementedException();
        }

        public static IQuery setParameters(this IQuery query, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            throw new NotImplementedException();
        }
    }
}
