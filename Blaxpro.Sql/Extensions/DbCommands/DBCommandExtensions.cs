using System.Collections.Generic;
using System.Data;

namespace Blaxpro.Sql.Extensions.DbCommands
{
    public static class DBCommandExtensions
    {
        public static IEnumerable<IDataRecord> getRecords(this IDbCommand command)
        {
            using (IDataReader reader = command.ExecuteReader())
                while (reader.Read())
                    yield return reader;
        }

        public static T setParameters<T>(this T command, IEnumerable<KeyValuePair<string, object>> parameters)
            where T: class, IDbCommand
        {
            foreach (var parameter in parameters)
                prv_setParameter(command, parameter.Key, parameter.Value);

            return command;
        }

        public static T setParameter<T>(this T command, string name, object value)
            where T : class, IDbCommand
        {
            return prv_setParameter(command, name, value);
        }

        private static T prv_setParameter<T>(this T command, string name, object value)
            where T : class, IDbCommand
        {
            IDbDataParameter commandParameter = command.CreateParameter();
            commandParameter.ParameterName = name;
            commandParameter.Value = value;
            command.Parameters.Add(commandParameter);

            return command;
        }
    }
}
