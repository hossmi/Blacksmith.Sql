using System;
using System.Runtime.Serialization;

namespace Blaxpro.Sql.Exceptions
{
    public class MigrationsSetupException : DbMigrationException
    {
        public MigrationsSetupException(string message) : base(message)
        {
        }

        public MigrationsSetupException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MigrationsSetupException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}